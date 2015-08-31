namespace Spreadsheet.Core
{
    internal class ExpressionParsingException : SpreadsheetException
    {
        public ExpressionParsingException(string message) : base(message) { }
    }
}
