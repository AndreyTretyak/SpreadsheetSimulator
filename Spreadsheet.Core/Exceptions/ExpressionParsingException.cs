using System;

namespace Spreadsheet.Core
{
    public class ExpressionParsingException : SpreadsheetException
    {
        public ExpressionParsingException(string message) : base(message)
        {
        }

        public ExpressionParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
