using System;
using System.Text;

using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Utils;

namespace Spreadsheet.Core;

public class SpreadsheetException : Exception
{

    protected SpreadsheetException(string message) : base(message) { }

    protected SpreadsheetException(string message, Exception innerException) : base(message, innerException) { }


    protected SpreadsheetException()
    {
    }

    protected StringBuilder _cellCallStack = new StringBuilder();

    public string MessageWithCellCallStack => $"{Message} {(InnerException as SpreadsheetException)?._cellCallStack}";

    public static T AddCellAddressToErrorStack<T>(T exception, CellAddress address) where T : SpreadsheetException
    {
        if (exception._cellCallStack.Length != 0)
        {
            var result = (T)Activator.CreateInstance(exception.GetType(), args: new object[] { exception.Message, exception });
            result._cellCallStack.Append(CellAddressConverter.GetString(address));
            result._cellCallStack.Append("<");
            result._cellCallStack.Append(exception._cellCallStack);
            return result;
        }
        else
        {
            exception._cellCallStack.Append(CellAddressConverter.GetString(address));
            return exception;
        }
    }
}