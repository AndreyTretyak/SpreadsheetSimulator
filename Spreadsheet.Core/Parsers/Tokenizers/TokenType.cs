namespace Spreadsheet.Core.Parsers.Tokenizers;

internal enum TokenType
{
    ExpressionStart,
    CellReference,
    Integer,
    String,
    Operator,
    LeftParenthesis,
    RightParenthesis,
    EndOfCell,
    EndOfStream
}