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

        public IExpression Expression { get; }

        public Cell(CellAddress address, IExpression expression)
        {
            Address = address;
            Expression = expression;
        }

        public ExpressionValue Evaluate(SpreedsheetProcessor processor, string callStack = null)
        {
            if (Expression == null)
                return new ExpressionValue(CellValueType.Nothing, null);

            if (callStack == null)
                callStack = string.Empty;

            if (callStack.Contains(Address.StringValue))
                return new ExpressionValue(CellValueType.Error, string.Format(Resources.CircularReferenceDetected, Address.StringValue));
            
            callStack += Address.StringValue + ParserSettings.CallStackSeparator;

            return Expression.Evaluate(processor, callStack);
        }
    }
}
