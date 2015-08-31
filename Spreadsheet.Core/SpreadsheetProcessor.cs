using System;
using System.Runtime.Caching;
using System.Threading;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.ProcessingStrategies;
using Spreadsheet.Core.Utils;

namespace Spreadsheet.Core
{
    public class SpreadsheetProcessor
    {
        private readonly Spreadsheet _spreadsheet;

        private readonly ExtendedLazy<Cell, object>[,] _memoryCache;

        public SpreadsheetProcessor(Spreadsheet spreadsheet)
        {
            _spreadsheet = spreadsheet;
            _memoryCache = new ExtendedLazy<Cell, object>[spreadsheet.RowCount, spreadsheet.ColumnCount];
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
            if (!cell.IsCashingRequered)
                return EvaluateCell(cell);

            if (_memoryCache[cell.Address.Row, cell.Address.Column] == null)
            {
                _memoryCache[cell.Address.Row, cell.Address.Column] = new ExtendedLazy<Cell,object>(cell, EvaluateCell);
            }
            return _memoryCache[cell.Address.Row, cell.Address.Column].Value;
        }

        private object EvaluateCell(Cell cell)
        {
            try
            {
                return cell.Evaluate(this);
            }
            catch (SpreadsheetException exception)
            {
                return  exception;
            }
            catch (Exception exception)
            {
                return new ExpressionEvaluationException(exception.Message, exception);
            }
        }
    }
}