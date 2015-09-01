using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading;
using Microsoft.CodeAnalysis.Collections;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Cells.Expressions;
using Spreadsheet.Core.Exceptions;
using Spreadsheet.Core.ProcessingStrategies;
using Spreadsheet.Core.Utils;

namespace Spreadsheet.Core
{
    public class SpreadsheetProcessor
    {
        private readonly Spreadsheet _spreadsheet;

        private readonly ExtendedLazy<Cell, object>[,] _memoryCache;

        private readonly Func<Cell, object> _evaluateCellFunct; 

        public SpreadsheetProcessor(Spreadsheet spreadsheet)
        {
            _spreadsheet = spreadsheet;
            _memoryCache = new ExtendedLazy<Cell, object>[spreadsheet.RowCount, spreadsheet.ColumnCount];
            _evaluateCellFunct = EvaluateCell;
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
                _memoryCache[cell.Address.Row, cell.Address.Column] = new ExtendedLazy<Cell,object>(cell, _evaluateCellFunct);
            }
            return _memoryCache[cell.Address.Row, cell.Address.Column].Value;
        }

        private object EvaluateCell(Cell cell)
        {
            try
            {
                var hashset = PooledHashSet<CellAddress>.GetInstance();
                try
                {
                    CheckRecursion(cell, new HashSet<CellAddress>());
                }
                finally
                {
                    hashset.Free();
                }
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

        private void CheckRecursion(Cell current, ISet<CellAddress> stack)
        {
            try
            {
                var addresses = PooledHashSet<CellAddress>.GetInstance();
                try
                {
                    GetDependencies(current.Expression, addresses);
                    if (addresses.Overlaps(stack))
                        throw new CircularCellRefereceException(Resources.CircularReference);

                    stack.Add(current.Address);
                    foreach (var address in addresses)
                    {
                        CheckRecursion(_spreadsheet[address], stack);
                    }
                    stack.Remove(current.Address);
                }
                finally
                {
                    addresses.Free();
                }
            }
            catch (SpreadsheetException ex)
            {
                throw SpreadsheetException.SetCellAddressToStack(ex, current.Address);
            }

        }

        private static void GetDependencies(IExpression expression, ISet<CellAddress> addresses)
        {
            var binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null)
            {
                GetDependencies(binaryExpression.Left, addresses);
                GetDependencies(binaryExpression.Right, addresses);
            }

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
            {
                GetDependencies(unaryExpression.Value, addresses);
            }

            var refereceExpression = expression as CellRefereceExpression;
            if (refereceExpression != null)
            {
                addresses.Add(refereceExpression.Address);
            }
        }
    }
}