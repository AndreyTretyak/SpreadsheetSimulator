using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spreadsheet.Core.Cells;

namespace Spreadsheet.Core.ExpressionParsers
{
    internal class SpreadsheetStreamParser
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
        }

        private Token Peek()
        {
            return _tokenizer.Peek();
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
            return new ConstantExpression(_tokenizer.Next().Integer);            
        }

        private IExpression ReadString()
        {
            return new ConstantExpression(_tokenizer.Next().String);
        }

        private IExpression ReadCellReference()
        {
            return new CellRefereceExpression(_tokenizer.Next().Address);
        }
        
        private IExpression ReadExpression()
        {
            Next();
            return ReadOperation();
        }
        
        private IExpression ReadOperation(int priority = 0)
        {
            if (_tokenizer.OperatorManager.Priorities.Count <= priority)
                return ReadIdentifier();

            var expression = ReadOperation(priority + 1);
            while (Peek(TokenType.Operator)
                   && Peek().Operator.Priority == _tokenizer.OperatorManager.Priorities.ElementAt(priority))
            {
                expression = new BinaryExpression(expression, Next().Operator, ReadOperation(priority + 1));
            }
            return expression;
        }

        private IExpression ReadIdentifier()
        {
            if (Peek(TokenType.LeftParanthesis))
            {
                Next();
                var expresion = ReadOperation();
                if (!Peek(TokenType.RightParanthesis))
                    throw InvalidContent(string.Format(Resources.WrongTokenType, ParserSettings.RightParanthesis));
                Next();
                return expresion;
            }

            switch (Peek().Type)
            {
                case TokenType.Operator:
                    return new UnaryExpression(Next().Operator, ReadIdentifier());
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
            return new ExpressionParsingException($"{string.Format(Resources.InvalidCellContent, _tokenizer.Next().String)} {additionMessage}");
        }
    }
}