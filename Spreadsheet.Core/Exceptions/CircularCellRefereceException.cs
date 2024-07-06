using System;

namespace Spreadsheet.Core.Exceptions;

public class CircularCellRefereceException : SpreadsheetException
{
    public CircularCellRefereceException(string message) : base(message)
    {
    }

    public CircularCellRefereceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
