using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Parsers.Operators;

namespace Spreadsheet.Core.Parsers.Tokenizers;

internal readonly struct Token
{
    public TokenType Type { get; }

    public string String { get; }

    public int Integer { get; }

    public CellAddress Address { get; }

    public IOperator Operator { get; }

    public Token(TokenType type) : this()
    {
        Type = type;
    }

    public Token(string value) : this(TokenType.String)
    {
        String = value;
    }

    public Token(int value) : this(TokenType.Integer)
    {
        Integer = value;
    }

    public Token(CellAddress value) : this(TokenType.CellReference)
    {
        Address = value;
    }

    public Token(IOperator value) : this(TokenType.Operator)
    {
        Operator = value;
    }

    public override string ToString()
    {
        object value = String;
        switch (Type)
        {
            case TokenType.Integer:
                value = Integer;
                break;
            case TokenType.CellReference:
                value = Address;
                break;
            case TokenType.Operator:
                value = Operator;
                break;
        }
        return value == null ? Type.ToString() : $"{Type}|{value}";
    }
}