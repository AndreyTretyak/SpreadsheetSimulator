using System;

namespace Spreadsheet.Core.Parsers.Operators;

internal class Operator<T>(char operatorCharacter, int priority, Func<T, T, T> binaryOperation = null, Func<T, T> unaryOperator = null) : IOperator
{
    public int Priority { get; } = priority;

    public char OperatorCharacter { get; } = operatorCharacter;

    private readonly Func<T, T, T> _binaryOperation = binaryOperation;

    private readonly Func<T, T> _unaryOperation = unaryOperator;

    public bool IsBinaryOperationSupported => _binaryOperation != null;

    public bool IsUnaryOperationSupported => _unaryOperation != null;

    public object BinaryOperation(object left, object right)
    {
        return IsBinaryOperationSupported
            ? (object)_binaryOperation(Cast(left), Cast(right))
            : throw new ExpressionEvaluationException(string.Format(Resources.BinaryOperationNotSupported, OperatorCharacter));
    }

    public object UnaryOperation(object value)
    {
        return IsUnaryOperationSupported
            ? (object)_unaryOperation(Cast(value))
            : throw new ExpressionEvaluationException(string.Format(Resources.UnaryOperationNotSupported, OperatorCharacter));
    }

    private T Cast(object value)
    {
        return typeof(T) != value?.GetType()
            ? throw new ExpressionEvaluationException(string.Format(Resources.WrongTypeError, typeof(T), value?.GetType()))
            : (T)value;
    }

    public override string ToString() => OperatorCharacter.ToString();
}