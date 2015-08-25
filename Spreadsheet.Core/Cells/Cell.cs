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
    public interface ICell
    {
        CellAddress Address { get; }

        object Evaluate(ISpreadsheetProcessor processor);
    }

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

    //public class EvaluatedCell : ICell
    //{
    //    public CellAddress Address { get; }

    //    public object Value { get; }

    //    public EvaluatedCell(CellAddress address, object value)
    //    {
    //        Address = address;
    //        Value = value;
    //    }

    //    public object Evaluate(ISpreadsheetProcessor processor) => Value;
    //}
}
