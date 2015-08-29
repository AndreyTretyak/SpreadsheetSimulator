namespace Spreadsheet.Core.Cells.Expressions
{
    public interface IExpression
    {
        object Evaluate(SpreadsheetProcessor processor);
    }
}