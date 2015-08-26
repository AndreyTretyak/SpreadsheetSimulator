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
        public IEnumerable<object> Evaluate(ISpreadsheet spreadsheet, Func<ICell,object> evaluation)
        {
           return spreadsheet.AsParallel()
                             .WithDegreeOfParallelism(spreadsheet.ColumnCount)
                             .OrderBy(e => e.Address.Row)
                             .ThenBy(e => e.Address.Column)
                             .Select(evaluation);
        }
    }
}
