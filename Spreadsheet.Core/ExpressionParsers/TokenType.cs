namespace Spreadsheet.Core.ExpressionParsers
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
        Unknown,
        EndOfStream
    }
}