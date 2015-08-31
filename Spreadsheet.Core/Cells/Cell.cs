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

        private readonly IExpression _expression;
        
        public Cell(CellAddress address, IExpression expression)
        {
            Address = address;
            _expression = expression;
        }
        
        public object Evaluate(SpreadsheetProcessor processor)
        {
            return _expression.Evaluate(processor);
        }

        public bool IsCashingRequered => !(_expression is ConstantExpression);

        public override string ToString() => $"{Address}|{_expression}";
    }
}
