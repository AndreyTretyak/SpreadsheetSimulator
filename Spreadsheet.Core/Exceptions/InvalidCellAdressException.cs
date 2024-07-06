using System;

namespace Spreadsheet.Core;

internal class InvalidCellAdressException : SpreadsheetException
{
    public InvalidCellAdressException(string message) : base(message)
    {
    }

    public InvalidCellAdressException(string message, Exception innerException) : base(message, innerException)
    {
    }
}