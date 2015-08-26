using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Spreadsheet.Core.ExpressionParsers;

namespace Spreadsheet.Core.Cells
{
    public class Cell : ICell
    {
        public CellAddress Address { get; }

        private IExpression Expression { get; }
        
        public Cell(CellAddress address, IExpression expression)
        {
            Address = address;
            Expression = expression;
        }
        

        public object Evaluate(ISpreadsheetProcessor processor)
        {
            return Expression?.Evaluate(processor);
        }

        public override string ToString() => $"{Address}|{Expression}";
    }
}
