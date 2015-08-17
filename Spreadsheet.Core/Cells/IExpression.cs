namespace SpreadsheetProcessor.Cells
{
    public interface IExpression
    {
        ExpressionValue Evaluate(ISpreadsheet processor, string callStack);
    }
}