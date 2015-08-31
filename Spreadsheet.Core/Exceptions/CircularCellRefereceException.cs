using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spreadsheet.Core.Exceptions
{
    public class CircularCellRefereceException : SpreadsheetException
    {
        public CircularCellRefereceException(string message) : base(message)
        {
        }

        public CircularCellRefereceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
