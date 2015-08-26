using System;
using System.Collections.Generic;
using System.Linq;
using Spreadsheet.Core.Cells;

namespace Spreadsheet.Core
{
    public class SimpleProcessingStrategy : IProcessingStrategy
    {
        public IEnumerable<object> Evaluate(ISpreadsheet spreadsheet, Func<Cell, object> evaluation)
        {
            return spreadsheet.Select(evaluation);
        }
    }
}