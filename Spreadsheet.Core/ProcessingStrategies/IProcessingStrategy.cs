using System;
using System.Collections.Generic;
using Spreadsheet.Core.Cells;

namespace Spreadsheet.Core.ProcessingStrategies;

public interface IProcessingStrategy
{
    IEnumerable<object> Evaluate(Spreadsheet spreadsheet, Func<Cell, object> evaluation);
}