namespace SpreadsheetProcessor.ExpressionParsers
{
    internal enum TokenType
    {
        ExpressionStart,
        CellReference,
        Integer,
        String,
        Operator,
        Unknown
    }
}