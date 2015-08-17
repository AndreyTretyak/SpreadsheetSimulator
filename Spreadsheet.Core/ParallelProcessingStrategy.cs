using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetProcessor.Cells;

namespace SpreadsheetProcessor
{
    public interface IProcessingStrategy
    {
        IEnumerable<ExpressionValue> Process(ISpreadsheet spreadsheet);
    }

    public class ParallelProcessingStrategy : IProcessingStrategy
    {
        public IEnumerable<ExpressionValue> Process(ISpreadsheet spreadsheet)
        {
           return spreadsheet.GetCells()
                             .AsParallel()
                             .WithDegreeOfParallelism(spreadsheet.MaxAddress.Column)
                             .OrderBy(e => e.Address.Row)
                             .ThenBy(e => e.Address.Column)
                             .Select(c => c.Evaluate(spreadsheet));
        }
    }

    public class SimpleProcessingStrategy : IProcessingStrategy
    {
        public IEnumerable<ExpressionValue> Process(ISpreadsheet spreadsheet)
        {
            return spreadsheet.GetCells()
                              .Select(c => c.Evaluate(spreadsheet));
        }
    }
}
