using System;

namespace Spreadsheet.Core
{
    public abstract class SpreadsheetException : SystemException
    {
        protected SpreadsheetException(string message) : base(message) { }

        protected SpreadsheetException(string message, Exception innerException) : base(message, innerException) { }

        public override string ToString() => Message;
    }
}