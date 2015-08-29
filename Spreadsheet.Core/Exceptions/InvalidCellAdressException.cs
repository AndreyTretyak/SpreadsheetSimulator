namespace Spreadsheet.Core
{
    internal class InvalidCellAdressException : SpreadsheetException
    {
        public InvalidCellAdressException(string message) : base(message) { }
    }
}