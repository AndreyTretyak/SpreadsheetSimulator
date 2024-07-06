using System;

namespace Spreadsheet.Core;

public class SpreadsheetReadingException : SpreadsheetException
{
    public SpreadsheetReadingException(string message) : base(message) { }

    public SpreadsheetReadingException(string message, Exception innerException) : base(message, innerException) { }
}