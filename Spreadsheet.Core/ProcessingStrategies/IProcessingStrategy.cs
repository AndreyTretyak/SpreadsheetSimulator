using System;
using System.Collections.Generic;
using Spreadsheet.Core.Cells;

namespace Spreadsheet.Core
{
    public interface IProcessingStrategy
    {
        IEnumerable<object> Evaluate(ISpreadsheet spreadsheet, Func<Cell, object> evaluation);
    }
}