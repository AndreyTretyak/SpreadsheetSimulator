using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SpreadsheetProcessor.Cells;
using SpreadsheetProcessors;

namespace SpreadsheetProcessor.ExpressionParsers
{
    interface ISpreadsheetSourse : IDisposable
    {
        IExpression NextExpression();
    }

    internal class SpreadsheetStreamParser : ISpreadsheetSourse
    {
        private readonly ISpreadsheetTokenizer _tokenizer;

        public SpreadsheetStreamParser(ISpreadsheetTokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        public IExpression NextExpression()
        {
            if (Peek(TokenType.EndOfStream))
                return null;

            var result = ReadCellContent();
            if (Peek(TokenType.EndOfExpression) || Peek(TokenType.EndOfStream))
            {
                    Next();
                    return result;
            }

            throw InvalidContent(string.Format(Resources.WrongTokenType, Resources.EndOfExpression));
            //while (!Peek(TokenType.EndOfStream))
            //{
            //    var result = ReadCellContent();
            //    if (Peek(TokenType.EndOfExpression) || Peek(TokenType.EndOfStream))
            //    {
            //        Next();
            //        yield return result;
            //    }
            //    else
            //    {
            //        yield return InvalidContent(string.Format(Resources.WrongTokenType, Resources.EndOfExpression));
            //    }
            //}
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
                    throw InvalidContent(Resources.UnknownContentType);
            }
        }

        private IExpression ReadNothing()
        {
            _tokenizer.Next();
            return new ConstantExpression(null);
        }

        private IExpression ReadInteger()
        {
            int result;
            var text = _tokenizer.Next().Value;
            if (int.TryParse(text, out result))
                return new ConstantExpression(result);
            throw new ExpressionParsingException(string.Format(Resources.FailedToParseNumber, text));
            
        }

        private IExpression ReadString()
        {
            return new ConstantExpression(_tokenizer.Next().Value);
        }

        private IExpression ReadCellReference()
        {
            return new CellRefereceExpression(new CellAddress(_tokenizer.Next().Value));
        }
        
        private IExpression ReadExpression()
        {
            Next();
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
                var expresion = ReadSum();
                if (!Peek(TokenType.RightParanthesis))
                    throw InvalidContent(string.Format(Resources.WrongTokenType, ParserSettings.RightParanthesis));
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
                return new BinaryExpression(new ConstantExpression(-1), 
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
                    throw InvalidContent(Resources.UnknownContentType);
            }
        }

        private ExpressionParsingException InvalidContent(string additionMessage)
        {
            return new ExpressionParsingException($"{string.Format(Resources.InvalidCellContent, _tokenizer.Next().Value)} {additionMessage}");
        }

        public void Dispose()
        {
            _tokenizer?.Dispose();
        }
    }
}