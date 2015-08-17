using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using SpreadsheetProcessors;

namespace SpreadsheetProcessor.Cells
{
    public class Cell
    {
        public CellAddress Address { get; }

        private IExpression Expression { get; }

        private ExpressionValue _calculatedValue;

        public Cell(CellAddress address, IExpression expression)
        {
            Address = address;
            Expression = expression;
        }

        public ExpressionValue Evaluate(ISpreadsheet processor, string callStack = null, bool reevaluate = false)
        {
            if (!reevaluate && _calculatedValue != null)
                return _calculatedValue;
            return _calculatedValue = InternalEvaluate(processor, callStack);
        }

        private ExpressionValue InternalEvaluate(ISpreadsheet processor, string callStack = null)
        {
            if (Expression == null)
                return new ExpressionValue(CellValueType.Nothing, null);

            if (callStack == null)
                callStack = string.Empty;

            var key = Address.StringValue + ParserSettings.CallStackSeparator;
            if (callStack.Contains(key))
                return new ExpressionValue(CellValueType.Error, string.Format(Resources.CircularReferenceDetected, Address.StringValue));

            callStack += key;

            return Expression.Evaluate(processor, callStack);
        }

        public override string ToString() => $"{Address}|{Expression}|{_calculatedValue}";
    }
}
