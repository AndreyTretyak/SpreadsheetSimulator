using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using SpreadsheetProcessor.ExpressionParsers;
using SpreadsheetProcessors;

namespace SpreadsheetProcessor.Cells
{
    public class Cell
    {
        public CellAddress Address { get; }

        private IExpression Expression { get; }
        
        public Cell(CellAddress address, IExpression expression)
        {
            Address = address;
            Expression = expression;
        }
        

        public object Evaluate(ISpreadsheet processor, string callStack = null)
        {
            if (Expression == null)
                return null;

            if (callStack == null)
                callStack = string.Empty;

            var key = Address.StringValue + ParserSettings.CallStackSeparator;
            if (callStack.Contains(key))
                throw new ExpressionEvaluationException(string.Format(Resources.CircularReferenceDetected, Address.StringValue));

            callStack += key;

            return Expression.Evaluate(processor, callStack);
        }

        public override string ToString() => $"{Address}|{Expression}";
    }
}
