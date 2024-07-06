using System;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.ProcessingStrategies;
using Spreadsheet.Core.Utils;
using Spreadsheet.Core.Validators;

namespace Spreadsheet.Core
{
    public class SpreadsheetProcessor
    {
        private readonly Spreadsheet _spreadsheet;

        private readonly ISpreadsheetValidator _validator;

        private readonly ExtendedLazy<Cell, object>[,] _memoryCache;

        private readonly Func<Cell, object> _evaluateCellFunct;


        public SpreadsheetProcessor(Spreadsheet spreadsheet, ISpreadsheetValidator validator = null)
        {
            _spreadsheet = spreadsheet;
            _memoryCache = new ExtendedLazy<Cell, object>[spreadsheet.RowCount, spreadsheet.ColumnCount];
            _evaluateCellFunct = EvaluateCell;
            _validator = validator ?? new RecursionDetectionValidator();
        }

        public SpreadsheetProcessingResult Evaluate()
        {
            return Evaluate(new SimpleProcessingStrategy());
        }

        public SpreadsheetProcessingResult Evaluate(IProcessingStrategy strategy)
        {
            return new SpreadsheetProcessingResult(_spreadsheet.ColumnCount, strategy.Evaluate(_spreadsheet, GetCellValue));
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
                _memoryCache[cell.Address.Row, cell.Address.Column] = new ExtendedLazy<Cell, object>(cell, _evaluateCellFunct);
            }
            return _memoryCache[cell.Address.Row, cell.Address.Column].Value;
        }

        private object EvaluateCell(Cell cell)
        {
            try
            {
                _validator?.Validate(_spreadsheet, cell);
                return cell.Evaluate(this);
            }
            catch (SpreadsheetException exception)
            {
                return exception;
            }
            catch (Exception exception)
            {
                return new ExpressionEvaluationException(exception.Message, exception);
            }
        }
    }
}