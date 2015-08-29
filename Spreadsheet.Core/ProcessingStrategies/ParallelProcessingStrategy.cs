using System;
using System.Collections.Generic;
using System.Linq;
using Spreadsheet.Core.Cells;

namespace Spreadsheet.Core.ProcessingStrategies
{
    public class ParallelProcessingStrategy : IProcessingStrategy
    {
        public IEnumerable<object> Evaluate(Spreadsheet spreadsheet, Func<Cell,object> evaluation)
        {
           return spreadsheet.AsParallel()
                             .AsOrdered()
                             .Select(evaluation);
        }
    }
}
