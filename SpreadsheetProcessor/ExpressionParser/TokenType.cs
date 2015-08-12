namespace SpreadsheetProcessor.ExpressionParser
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