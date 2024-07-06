using Spreadsheet.Core.Parsers.Operators;

namespace Spreadsheet.Core.Cells.Expressions;

internal class UnaryExpression : IExpression
{
    public IExpression Value { get; }

    public IOperator Operation { get; }

    public UnaryExpression(IOperator operation, IExpression value)
    {
        Value = value;
        Operation = operation;
    }

    public object Evaluate(SpreadsheetProcessor processor)
    {
        return Operation.UnaryOperation(Value.Evaluate(processor));
    }

    public override string ToString() => $"{Operation}{Value}";
}