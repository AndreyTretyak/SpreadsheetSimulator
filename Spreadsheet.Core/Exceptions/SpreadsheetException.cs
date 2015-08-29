using System;

namespace Spreadsheet.Core
{
    internal class SpreadsheetException : SystemException
    {
        public SpreadsheetException(string message) : base(message) { }

        public SpreadsheetException(string message, Exception innerException) : base(message, innerException) { }

        public override string ToString() => Message;
    }
}