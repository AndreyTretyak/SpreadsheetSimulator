using System;

namespace Spreadsheet.Core
{
    public class SpreadsheatReadingException : SpreadsheetException
    {
        public SpreadsheatReadingException(string message) : base(message)
        {
        }

        public SpreadsheatReadingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}