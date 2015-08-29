namespace Spreadsheet.Core.Parsers.Tokenizers
{
    internal enum TokenType
    {
        ExpressionStart,
        CellReference,
        Integer,
        String,
        Operator,
        LeftParanthesis,
        RightParanthesis,
        EndOfExpression,
        EndOfStream
    }
}