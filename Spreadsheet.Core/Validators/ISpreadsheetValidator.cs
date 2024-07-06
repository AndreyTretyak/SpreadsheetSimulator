using Spreadsheet.Core.Cells;

namespace Spreadsheet.Core.Validators;

public interface ISpreadsheetValidator
{
    void Validate(Spreadsheet spreadsheet, Cell cell);
}