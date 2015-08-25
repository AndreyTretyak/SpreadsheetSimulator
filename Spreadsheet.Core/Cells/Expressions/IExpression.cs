namespace SpreadsheetProcessor.Cells
{
    public interface IExpression
    {
        object Evaluate(ISpreadsheet processor);
    }
}