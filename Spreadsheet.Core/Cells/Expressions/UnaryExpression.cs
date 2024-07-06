using Spreadsheet.Core.Parsers.Operators;

namespace Spreadsheet.Core.Cells.Expressions;

internal class UnaryExpression(IOperator operation, IExpression value) : IExpression
{
    public IExpression Value { get; } = value;

    public IOperator Operation { get; } = operation;

    public object Evaluate(SpreadsheetProcessor processor) => Operation.UnaryOperation(Value.Evaluate(processor));

    public override string ToString() => $"{Operation}{Value}";
}