using System.Linq;
using Spreadsheet.Core.Cells.Expressions;
using Spreadsheet.Core.Parsers.Tokenizers;

namespace Spreadsheet.Core.Parsers
{
    internal class SpreadsheetStreamParser : ISpreadsheetParser
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
            if (Peek(TokenType.EndOfCell) || Peek(TokenType.EndOfStream))
            {
                Next();
                return result;
            }
            throw InvalidContent(Resources.WrongTokenType, Resources.EndOfExpression);
        }

        private Token Peek() => _tokenizer.Peek();

        private bool Peek(TokenType type) => Peek().Type == type;

        private Token Next() => _tokenizer.Next();

        private IExpression ReadCellContent()
        {
            switch (_tokenizer.Peek().Type)
            {
                case TokenType.EndOfCell:
                    return ReadNothing();
                case TokenType.Integer:
                    return ReadInteger();
                case TokenType.String:
                    return ReadString();
                case TokenType.ExpressionStart:
                    return ReadExpression();
                default:
                    throw InvalidContent(Resources.UnknownCellStart);
            }
        }

        private IExpression ReadNothing()
        {
            return new ConstantExpression(null);
        }

        private IExpression ReadInteger() => new ConstantExpression(_tokenizer.Next().Integer);

        private IExpression ReadString() => new ConstantExpression(_tokenizer.Next().String);

        private IExpression ReadCellReference() => new CellRefereceExpression(_tokenizer.Next().Address);

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
                   && Peek().Operator.Priority == _tokenizer.OperatorManager.Priorities[priority])
            {
                expression = new BinaryExpression(expression, Next().Operator, ReadOperation(priority + 1));
            }
            return expression;
        }

        private IExpression ReadIdentifier()
        {
            switch (Peek().Type)
            {
                case TokenType.LeftParenthesis:
                    Next();
                    var expresion = ReadOperation();
                    if (!Peek(TokenType.RightParenthesis))
                        throw InvalidContent(Resources.WrongTokenType, SpesialCharactersSettings.RightParanthesis);
                    Next();
                    return expresion;
                case TokenType.Operator:
                    return new UnaryExpression(Next().Operator, ReadIdentifier());
                case TokenType.Integer:
                    return ReadInteger();
                case TokenType.CellReference:
                    return ReadCellReference();
                default:
                    throw InvalidContent(Resources.UnknownExpressionElement);
            }
        }

        private ExpressionParsingException InvalidContent(string message, object expected = null)
        {
            return new ExpressionParsingException(string.Format(message, _tokenizer.Next(), expected));
        }
    }
}