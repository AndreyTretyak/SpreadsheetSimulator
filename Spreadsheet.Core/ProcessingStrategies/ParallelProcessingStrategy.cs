using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spreadsheet.Core.Cells;

namespace Spreadsheet.Core
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
