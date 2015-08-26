namespace Spreadsheet.Core.Cells
{
    public interface ICell
    {
        CellAddress Address { get; }

        object Evaluate(ISpreadsheetProcessor processor);
    }
}