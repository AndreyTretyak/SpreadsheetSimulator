namespace SpreadsheetProcessor.Cells
{
    public interface IExpression
    {
        object Evaluate(ISpreadsheetProcessor processor);
    }
}