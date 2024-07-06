namespace Spreadsheet.Core.Parsers.Operators;

internal interface IOperator
{
    int Priority { get; }

    char OperatorCharacter { get; }

    bool IsBinaryOperationSupported { get; }

    bool IsUnaryOperationSupported { get; }

    object BinaryOperation(object left, object right);

    object UnaryOperation(object value);
}