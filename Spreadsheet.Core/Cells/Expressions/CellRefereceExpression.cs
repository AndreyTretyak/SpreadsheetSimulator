using System;
using System.Collections.Generic;
using System.Linq;

namespace Spreadsheet.Core.Cells.Expressions
{
    internal class CellRefereceExpression : IExpression
    {
        public CellAddress Address { get; }

        public CellRefereceExpression(CellAddress address)
        {
            Address = address;
        }

        public object Evaluate(SpreadsheetProcessor processor)
        {
            var result = processor.GetCellValue(Address);
            var exception = result as Exception;
            if (exception != null)
                throw exception;
            return result;
        }

        public override string ToString() => Address.ToString();
    }
}