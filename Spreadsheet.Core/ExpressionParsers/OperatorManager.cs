using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetProcessor.Cells;

namespace SpreadsheetProcessor.ExpressionParsers
{
    internal class OperatorManager
    {
        public List<BinaryOperator> BinaryOperators { get; }

        public List<PrefixOperator> PrefixOperators { get; }

        public OperatorManager()
        {
            PrefixOperators = new List<PrefixOperator>
            {
                new PrefixOperator(CellValueType.Integer, "+", v => v),
                new PrefixOperator(CellValueType.Integer, "-", v => -(int)v)
            };
            BinaryOperators = new List<BinaryOperator>
            {
                new BinaryOperator(1,CellValueType.Integer, "+", CellValueType.Integer, (f,s) => (int)f + (int)s),
                new BinaryOperator(1,CellValueType.Integer, "-", CellValueType.Integer, (f,s) => (int)f - (int)s),
                new BinaryOperator(2,CellValueType.Integer, "*", CellValueType.Integer, (f,s) => (int)f * (int)s),
                new BinaryOperator(2,CellValueType.Integer, "/", CellValueType.Integer, (f,s) => (int)f / (int)s),
                new BinaryOperator(2,CellValueType.Integer, "^", CellValueType.Integer, (f,s) => (int)Math.Pow((int)f,(int)s))
            };
        }
    }

    internal class BinaryOperator
    {
        public int Priority { get; }

        public string Operator { get; }

        public CellValueType LeftType { get; }

        public CellValueType RightType { get; }

        public Func<object, object, object> Operation { get; }

        public BinaryOperator(int priority, CellValueType left, string @operator, CellValueType right, Func<object, object, object> operation)
        {
            Priority = priority;
            LeftType = left;
            Operator = @operator;
            RightType = right;
            Operation = operation;
        }
    }

    internal class PrefixOperator
    {
        public CellValueType ValueType { get; }

        public string Operator { get; }

        public Func<object, object> Operation { get; }

        public PrefixOperator(CellValueType type, string @operator, Func<object, object> operation)
        {
            ValueType = type;
            Operator = @operator;
            Operation = operation;
        }
    }

}
