using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SpreadsheetProcessor.Cells;
using SpreadsheetProcessors;

namespace SpreadsheetProcessor.ExpressionParsers
{
    internal class ExpressionParser
    {
        private ExpressionTokenizer Tokenizer { get; } = new ExpressionTokenizer();
        
        public IExpression Parse(string expresion)
        {
            var tokens = Tokenizer.GetTokens(expresion).ToArray();
            switch (tokens.Length)
            {
                case 0:
                    return new ConstantExpression(new ExpressionValue(CellValueType.Nothing, null));
                case 1:
                    var token = tokens[0];
                    switch (token.Type)
                    {
                        case TokenType.Integer:
                            return ParseNumber(token);
                        case TokenType.String:
                            return ParseString(token);
                        default:
                            return InvalidContent(expresion, Resources.UnknownContentType);
                    }
                default:
                    return tokens[0].Type == TokenType.ExpressionStart 
                           ? ParseExpression(tokens)
                           : InvalidContent(expresion, string.Format(Resources.WrongExpressionStart, ParserSettings.ExpressionStart));
            }
        }

        private IExpression InvalidContent(string cellText, string additionMessage)
        {
            return new ConstantExpression(new ExpressionValue(CellValueType.Error, 
                string.Format(Resources.InvalidCellContent, cellText) + " " + additionMessage));
        }

        private IExpression ParseNumber(Token token)
        {
            int result;
            return int.TryParse(token.Value, out result) 
                ? new ConstantExpression(new ExpressionValue(CellValueType.Integer,  result)) 
                : new ConstantExpression(new ExpressionValue(CellValueType.Error, string.Format(Resources.FailedToParseNumber, token.Value)));
        }

        private IExpression ParseString(Token token)
        {
            return new ConstantExpression(new ExpressionValue(CellValueType.String, token.Value));
        }

        private IExpression ParseCellReference(Token token)
        {
            var referece = new CellAddress(token.Value);
            return new CellRefereceExpression(referece);
        }

        private IExpression ParseExpression(IReadOnlyList<Token> tokens, int index = 1)
        {
            IExpression left;
            switch (tokens[index].Type)
            {
                case TokenType.Integer:
                    left = ParseNumber(tokens[index]);
                    break;
                case TokenType.CellReference:
                    left = ParseCellReference(tokens[index]);
                    break;
                default:
                    return InvalidContent(tokens[index].Value, Resources.IdentifierExpected);
            }
            if (index + 1 >= tokens.Count)
                return left;

            index++;
            if (tokens[index].Type != TokenType.Operator)
                return InvalidContent(tokens[index].Value, Resources.OperatorExpected);

            var operation = tokens[index].Value;

            if (index + 1 >= tokens.Count)
                return InvalidContent(tokens[index].Value, Resources.IdentifierExpected);

            return new BinaryExpression(left, operation, ParseExpression(tokens, index + 1));
        }
    }

    internal class ExpressionParserNew
    {
        private readonly ExpressionTokenizerStream _tokenizer;

        public ExpressionParserNew(StreamReader stream)
        {
            _tokenizer = new ExpressionTokenizerStream(stream);
        }

        public ExpressionParserNew(Stream stream)
        {
            _tokenizer = new ExpressionTokenizerStream(stream);
        }

        public IEnumerable<IExpression> GetExpressions()
        {
            while (!Peek(TokenType.EndOfStream))
            {
                var result = ReadCellContent();
                if (Peek(TokenType.EndOfExpression))
                {
                    Next();
                    yield return result;
                }
                else
                {
                    yield return InvalidContent(string.Format(Resources.WrongTokenType, Resources.EndOfExpression));
                }
            }
        }

        private Token Peek()
        {
            return _tokenizer.Peek();
        }

        private bool Peek(string value)
        {
            return Peek().Value == value;
        }

        private bool Peek(TokenType type)
        {
            return Peek().Type == type;
        }

        private Token Next()
        {
            return _tokenizer.Next();
        }

        private IExpression ReadCellContent()
        {
            switch (_tokenizer.Peek().Type)
            {
                case TokenType.EndOfExpression:
                    return ReadNothing();
                case TokenType.Integer:
                    return ReadInteger();
                case TokenType.String:
                    return ReadString();
                case TokenType.ExpressionStart:
                    return ReadExpression();
                default:
                    return InvalidContent(Resources.UnknownContentType);
            }
        }

        public IExpression ReadNothing()
        {
            _tokenizer.Next();
            return new ConstantExpression(new ExpressionValue(CellValueType.Nothing, null));
        }

        private IExpression ReadInteger()
        {
            int result;
            var text = _tokenizer.Next().Value;
            var value = int.TryParse(text, out result)
                ? new ExpressionValue(CellValueType.Integer, result)
                : new ExpressionValue(CellValueType.Error, string.Format(Resources.FailedToParseNumber, text));
            return new ConstantExpression(value);
        }

        private IExpression ReadString()
        {
            return new ConstantExpression(new ExpressionValue(CellValueType.String, _tokenizer.Next().Value));
        }

        private IExpression ReadCellReference()
        {
            var referece = new CellAddress(_tokenizer.Next().Value);
            return new CellRefereceExpression(referece);
        }
        
        private IExpression ReadExpression()
        {
            return ReadSum();
        }

        private IExpression ReadSum()
        {
            var binaryExpression = new BinaryExpression(ReadFract());
            while (Peek("+") || Peek("-"))
            {
                if (binaryExpression.Right != null)
                {
                    binaryExpression = new BinaryExpression(binaryExpression);
                }
                binaryExpression.Operation = Next().Value;
                binaryExpression.Right = ReadFract();
            }
            return binaryExpression;
        }

        private IExpression ReadFract()
        {
            var binaryExpression = new BinaryExpression(ReadIdentifier());
            while (Peek("*") || Peek("/") || Peek("%"))
            {
                if (binaryExpression.Right != null)
                {
                    binaryExpression = new BinaryExpression(binaryExpression);
                }
                binaryExpression.Operation = Next().Value;
                binaryExpression.Right = ReadIdentifier();
            }
            return binaryExpression;
        }

        private IExpression ReadIdentifier()
        {
            if (Peek(TokenType.LeftParanthesis))
            {
                Next();
                var expresion = ReadExpression();
                if (Peek(TokenType.RightParanthesis))
                    return InvalidContent(string.Format(Resources.WrongTokenType, ParserSettings.RightParanthesis));
                Next();
                return expresion;
            }
            
            //Stub for prefix addition operator
            if (Peek(ParserSettings.AdditionOperator))
            {
                Next();
                return ReadIdentifier();
            }
            
            //Stub for prefix subtraction operator
            if (Peek(ParserSettings.SubtractionOperator))
            {
                Next();
                return new BinaryExpression(new ConstantExpression(CellValueType.Integer, -1), 
                                            ParserSettings.MultiplicationOperator, 
                                            ReadIdentifier());
            }
            
            switch (Peek().Type)
            {
                case TokenType.Integer:
                    return ReadInteger();
                case TokenType.CellReference:
                    return ReadCellReference();
                default:
                    return InvalidContent(Resources.UnknownContentType);
            }
        }

        private IExpression InvalidContent(string additionMessage)
        {
            return new ConstantExpression(new ExpressionValue(CellValueType.Error,
                string.Format(Resources.InvalidCellContent, _tokenizer.Next().Value) + " " + additionMessage));
        }
    }
}