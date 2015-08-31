using System;
using System.CodeDom;
using System.Runtime.InteropServices;
using System.Text;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Utils;

namespace Spreadsheet.Core
{
    public class SpreadsheetException : SystemException
    {

        protected SpreadsheetException(string message) : base(message) { }

        protected SpreadsheetException(string message, Exception innerException) : base(message, innerException) { }


        protected SpreadsheetException()
        {
        }

        protected CellAddress? _cellAddress;

        public string MessageWithCallStack => $"{Message} {GetCellCallStack()}";

        public string GetCellCallStack()
        {
            var builder = new StringBuilder();
            if (_cellAddress.HasValue)
            {
                builder.Append(CellAddressConverter.GetString(_cellAddress.Value));
            }
            
            var inner = InnerException as SpreadsheetException;
            if (inner != null)
            {
                var stack = inner.GetCellCallStack();
                if (!string.IsNullOrWhiteSpace(stack))
                {
                    builder.Append("<");
                    builder.Append(stack);
                }
            }
            return builder.ToString();
        }

        public static T SetCellAddressToStack<T>(T exception, CellAddress address) where T : SpreadsheetException
        {
            T result;
            if (exception._cellAddress.HasValue)
            {
                result = (T)Activator.CreateInstance(exception.GetType(), args: new object[] { exception.Message, exception});
            }
            else
            {
                result = exception;
            }
            result._cellAddress = address;
            return result;
        }
    }
}