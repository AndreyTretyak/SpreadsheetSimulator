using System;

namespace Spreadsheet.Core;

internal class InvalidCellAddressException : SpreadsheetException
{
    public InvalidCellAddressException(string message) : base(message) { }

    public InvalidCellAddressException(string message, Exception innerException) : base(message, innerException) { }
}