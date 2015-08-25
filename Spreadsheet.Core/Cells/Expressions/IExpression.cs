namespace Spreadsheet.Core.Cells
{
    public interface IExpression
    {
        object Evaluate(ISpreadsheetProcessor processor);
    }
}