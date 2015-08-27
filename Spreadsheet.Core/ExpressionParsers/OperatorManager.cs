using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spreadsheet.Core.Cells;

namespace Spreadsheet.Core.ExpressionParsers
{
    internal class OperatorManager
    {
        public List<IBinaryOperator> BinaryOperators { get; }

        public List<IPrefixOperator> PrefixOperators { get; }

        public OperatorManager()
        {
            PrefixOperators = new List<IPrefixOperator>
            {
                new PrefixOperator<int>("+", v => v),
                new PrefixOperator<int>("-", v => -v)
            };
            BinaryOperators = new List<IBinaryOperator>
            {
                new BinaryOperator<int,int>(1, "+", (l,r) => l + r),
                new BinaryOperator<int,int>(1, "-", (l,r) => l - r),
                new BinaryOperator<int,int>(2, "*", (l,r) => l * r),
                new BinaryOperator<int,int>(2, "/", (l,r) => l / r),
                new BinaryOperator<int,int>(3, "^", (l,r) => (int)Math.Pow(l,r))
            };
        }
    }

    public interface IBinaryOperator
    {
        object Evaluate(object left, object right);
    }

    internal class Operator
    {
        protected T Cast<T>(object value)
        {
            if (typeof(T) != value?.GetType())
                throw new ExpressionEvaluationException(string.Format(Resources.WrongTypeError, typeof(T), value?.GetType()));
            return (T)value;
        }
    }

    internal class BinaryOperator<TLeft,TRight> : Operator, IBinaryOperator
    {
        public int Priority { get; }

        public string Operator { get; }

        public Func<TLeft, TRight, object> Operation { get; }

        public BinaryOperator(int priority, string @operator, Func<TLeft, TRight, object> operation)
        {
            Priority = priority;
            Operator = @operator;
            Operation = operation;
        }

        public object Evaluate(object left, object right)
        {
            return Operation(Cast<TLeft>(left), Cast<TRight>(right));
        }
    }

    public interface IPrefixOperator
    {
        object Evaluate(object value);
    }

    internal class PrefixOperator<T> : Operator, IPrefixOperator
    {
        public string Operator { get; }

        public Func<T, object> Operation { get; }

        public PrefixOperator(string @operator, Func<T, object> operation)
        {
            Operator = @operator;
            Operation = operation;
        }

        public object Evaluate(object value)
        {
            return Operation(Cast<T>(value));
        }
    }
}
