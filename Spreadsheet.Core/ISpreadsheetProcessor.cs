namespace Spreadsheet.Core
{
    public interface ISpreadsheetProcessor
    {
        object GetCellValue(CellAddress address);
    }
}