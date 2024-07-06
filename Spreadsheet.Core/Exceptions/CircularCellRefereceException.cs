using System;

namespace Spreadsheet.Core.Exceptions;

public class CircularCellReferenceException : SpreadsheetException
{
    public CircularCellReferenceException(string message) : base(message) { }

    public CircularCellReferenceException(string message, Exception innerException) : base(message, innerException) { }
}
