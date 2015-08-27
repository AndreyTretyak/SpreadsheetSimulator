using System;
using System.Runtime.Caching;
using System.Threading;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.ExpressionParsers;

namespace Spreadsheet.Core
{
    public class SpreadsheetProcessor
    {
        private readonly Spreadsheet _spreadsheet;

        private readonly Lazy<object>[,] _memoryCache;

        public SpreadsheetProcessor(Spreadsheet spreadsheet)
        {
            _spreadsheet = spreadsheet;
            _memoryCache = new Lazy<object>[spreadsheet.RowCount, spreadsheet.ColumnCount];
        }

        public SpreadsheetEvaluationResult Evaluate(IProcessingStrategy strategy)
        {
            return new SpreadsheetEvaluationResult(_spreadsheet.ColumnCount, strategy.Evaluate(_spreadsheet, GetCellValue));
        }

        public object GetCellValue(CellAddress address)
        {
            return GetCellValue(_spreadsheet[address]);
        }

        private object GetCellValue(Cell cell)
        {
            if (_memoryCache[cell.Address.Row, cell.Address.Column] == null)
            {
                _memoryCache[cell.Address.Row, cell.Address.Column] = new Lazy<object>(() => EvaluateCell(cell), LazyThreadSafetyMode.ExecutionAndPublication);
            }
            return _memoryCache[cell.Address.Row, cell.Address.Column].Value;
        }

        private object EvaluateCell(Cell cell)
        {
            try
            {
                return cell.Evaluate(this);
            }
            catch (ExpressionEvaluationException exception)
            {
                return  exception;
            }
            //circular reference
            catch (InvalidOperationException exception)
            {
                return new ExpressionEvaluationException(Resources.CircularReferenceDetected, exception);
            }
            //TODO more exact types should be specified
            catch (Exception exception)
            {
                return new ExpressionEvaluationException(exception.Message, exception);
            }
        }
    }
}