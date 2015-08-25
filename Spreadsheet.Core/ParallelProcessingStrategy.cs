using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Spreadsheet.Core.Cells;

namespace Spreadsheet.Core
{
    public interface IEvaluationStrategy
    {
        IEnumerable<object> Evaluate(ISpreadsheet spreadsheet, Func<ICell, object> evaluation);
    }

    public class ParallelEvaluationStrategy : IEvaluationStrategy
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

    public class SimpleEvaluationStrategy : IEvaluationStrategy
    {
        public IEnumerable<object> Evaluate(ISpreadsheet spreadsheet, Func<ICell, object> evaluation)
        {
            return spreadsheet.Select(evaluation);
        }
    }

    public interface ISpreadsheetProcessor
    {
        object GetCellValue(CellAddress address);
    }

    public class SpreadsheetProcessor : ISpreadsheetProcessor
    {
        private readonly ISpreadsheet _spreadsheet;

        private readonly MemoryCache _memoryCache;

        public SpreadsheetProcessor(ISpreadsheet spreadsheet)
        {
            _spreadsheet = spreadsheet;
            _memoryCache = new MemoryCache("Spreadsheet");
        }

        public IEnumerable<object> Evaluate(IEvaluationStrategy strategy)
        {
            return strategy.Evaluate(_spreadsheet, GetCellValue);
        }

        private object GetCellValue(ICell cell)
        {
            var key = cell.Address.StringValue;
            var cacheValue = _memoryCache.Get(key);
            if (cacheValue != null)
                return ((Lazy<object>)cacheValue).Value;

            var value = new Lazy<object>(() => cell.Evaluate(this), LazyThreadSafetyMode.ExecutionAndPublication);
            _memoryCache.Add(key, value, DateTimeOffset.MaxValue);
            return value.Value;
        }

        public object GetCellValue(CellAddress address)
        {
            return GetCellValue(_spreadsheet[address]);
        }

    }
}
