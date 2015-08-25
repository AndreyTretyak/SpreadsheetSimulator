using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetProcessor.Cells;

namespace SpreadsheetProcessor
{
    public interface IEvaluationStrategy
    {
        IEnumerable<object> Evaluate(ISpreadsheet spreadsheet);
    }

    public class ParallelEvaluationStrategy : IEvaluationStrategy
    {
        public IEnumerable<object> Evaluate(ISpreadsheet spreadsheet)
        {
           return spreadsheet.AsParallel()
                             .WithDegreeOfParallelism(spreadsheet.MaxAddress.Column)
                             .OrderBy(e => e.Address.Row)
                             .ThenBy(e => e.Address.Column)
                             .Select(c => c.Evaluate(spreadsheet));
        }
    }

    public class SimpleEvaluationStrategy : IEvaluationStrategy
    {
        public IEnumerable<object> Evaluate(ISpreadsheet spreadsheet)
        {
            return spreadsheet.Select(c => c.Evaluate(spreadsheet));
        }
    }

    public class SpreadsheetEvaluator
    {
        public EvaluatedSpreadsheet Evaluate(ISpreadsheet spreadsheet, IEvaluationStrategy strategy)
        {
            return new EvaluatedSpreadsheet(spreadsheet.MaxAddress, strategy.Evaluate(spreadsheet));
        }
    }
}
