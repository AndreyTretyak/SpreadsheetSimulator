using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Spreadsheet.Core.Cells.Expressions;

namespace Spreadsheet.Core.Cells
{
    public class Cell
    {
        public CellAddress Address { get; }

        public bool IsCashingRequered { get; }

        private readonly IExpression _expression;

        internal Cell(CellAddress address, IExpression expression)
        {
            Address = address;
            _expression = expression;
            IsCashingRequered = !(_expression is ConstantExpression);
        }
        
        public object Evaluate(SpreadsheetProcessor processor)
        {
            try
            {
                return _expression.Evaluate(processor);
            }
            catch (SpreadsheetException exception)
            {
                throw SpreadsheetException.SetCellAddressToStack(exception, Address);
            }
            catch (Exception exception)
            {
                throw SpreadsheetException.SetCellAddressToStack(new ExpressionEvaluationException(exception.Message, exception), Address);
            }
        }

        public override string ToString() => $"{Address}|{_expression}";
    }
}
