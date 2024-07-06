using System;

namespace Spreadsheet.Core.Parsers.Operators
{
    internal class Operator<T> : IOperator
    {
        public int Priority { get; }

        public char OperatorCharacter { get; }

        private readonly Func<T, T, T> _binaryOperation;

        private readonly Func<T, T> _unaryOperation;

        public Operator(char operatorCharacter, int priority, Func<T, T, T> binaryOperation = null, Func<T, T> unaryOperator = null)
        {
            Priority = priority;
            OperatorCharacter = operatorCharacter;
            _binaryOperation = binaryOperation;
            _unaryOperation = unaryOperator;
        }

        public bool IsBinaryOperationSupported => _binaryOperation != null;

        public bool IsUnaryOperationSupported => _unaryOperation != null;

        public object BinaryOperation(object left, object right)
        {
            if (IsBinaryOperationSupported)
                return _binaryOperation(Cast(left), Cast(right));
            throw new ExpressionEvaluationException(string.Format(Resources.BinaryOperationNotSupported, OperatorCharacter));
        }

        public object UnaryOperation(object value)
        {
            if (IsUnaryOperationSupported)
                return _unaryOperation(Cast(value));
            throw new ExpressionEvaluationException(string.Format(Resources.UnaryOperationNotSupported, OperatorCharacter));
        }

        private T Cast(object value)
        {
            if (typeof(T) != value?.GetType())
                throw new ExpressionEvaluationException(string.Format(Resources.WrongTypeError, typeof(T), value?.GetType()));
            return (T)value;
        }

        public override string ToString()
        {
            return OperatorCharacter.ToString();
        }
    }
}