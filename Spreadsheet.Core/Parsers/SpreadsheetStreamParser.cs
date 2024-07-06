using Spreadsheet.Core.Cells.Expressions;
using Spreadsheet.Core.Parsers.Tokenizers;
using Spreadsheet.Core.Utils;

namespace Spreadsheet.Core.Parsers;

internal class SpreadsheetStreamParser : ISpreadsheetParser
{
    private readonly ISpreadsheetTokenizer _tokenizer;

    public SpreadsheetStreamParser(ISpreadsheetTokenizer tokenizer) => _tokenizer = tokenizer;

    public IExpression ReadExpression()
    {
        if (Peek(TokenType.EndOfStream))
            return null;

        var result = ReadCellContent();
        if (Peek(TokenType.EndOfCell) || Peek(TokenType.EndOfStream))
        {
            Read();
            return result;
        }
        throw InvalidContent(Resources.WrongTokenType, Resources.EndOfExpression);
    }

    private Token Peek() => _tokenizer.Peek();

    private bool Peek(TokenType type) => Peek().Type == type;

    private Token Read() => _tokenizer.Read();

    private IExpression ReadCellContent()
    {
        return _tokenizer.Peek().Type switch
        {
            TokenType.EndOfCell => ReadNothing(),
            TokenType.Integer => ReadInteger(),
            TokenType.String => ReadString(),
            TokenType.ExpressionStart => ReadComplexExpression(),
            _ => throw InvalidContent(Resources.UnknownCellStart),
        };
    }

    private IExpression ReadNothing() => new ConstantExpression(null);

    private IExpression ReadInteger() => new ConstantExpression(_tokenizer.Read().Integer);

    private IExpression ReadString() => new ConstantExpression(_tokenizer.Read().String);

    private IExpression ReadCellReference() => new CellReferenceExpression(_tokenizer.Read().Address);

    private IExpression ReadComplexExpression()
    {
        Read();
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
            expression = new BinaryExpression(expression, Read().Operator, ReadOperation(priority + 1));
        }
        return expression;
    }

    private IExpression ReadIdentifier()
    {
        switch (Peek().Type)
        {
            case TokenType.LeftParenthesis:
                Read();
                var expresion = ReadOperation();
                if (!Peek(TokenType.RightParenthesis))
                    throw InvalidContent(Resources.WrongTokenType, SpesialCharactersSettings.RightParathesis);
                Read();
                return expresion;
            case TokenType.Operator:
                return new UnaryExpression(Read().Operator, ReadIdentifier());
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
        return new ExpressionParsingException(string.Format(message, _tokenizer.Read(), expected));
    }
}