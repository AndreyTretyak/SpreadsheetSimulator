using System;
using System.Runtime.Caching;
using System.Threading;
using Spreadsheet.Core.Cells;

namespace Spreadsheet.Core
{
    public class SpreadsheetProcessor : ISpreadsheetProcessor
    {
        private readonly ISpreadsheet _spreadsheet;

        private readonly MemoryCache _memoryCache;

        public SpreadsheetProcessor(ISpreadsheet spreadsheet)
        {
            _spreadsheet = spreadsheet;
            _memoryCache = new MemoryCache("Spreadsheet");
        }

        public SpreadsheetEvaluationResult Evaluate(IProcessingStrategy strategy)
        {
            return new SpreadsheetEvaluationResult(_spreadsheet.ColumnCount, strategy.Evaluate(_spreadsheet, GetCellValue));
        }

        public object GetCellValue(CellAddress address)
        {
            return GetCellValue(_spreadsheet[address]);
        }

        private object GetCellValue(ICell cell)
        {
            var key = cell.Address.StringValue;
            var cacheValue = _memoryCache.Get(key);
            if (cacheValue != null)
                return ((Lazy<object>)cacheValue).Value;

            var value = new Lazy<object>(() => EvaluateCell(cell), LazyThreadSafetyMode.ExecutionAndPublication);
            _memoryCache.Add(key, value, DateTimeOffset.MaxValue);
            return value.Value;
        }

        private object EvaluateCell(ICell cell)
        {
            try
            {
                return cell.Evaluate(this);
            }
            //TODO more exact types should be specified
            catch (Exception exception)
            {
                return exception;
            }
        }
    }
}