namespace Spreadsheet.Core.Cells.Expressions;

internal class ConstantExpression(object value) : IExpression
{
    internal object Value { get; } = value;

    public object Evaluate(SpreadsheetProcessor processor) => Value;

    public override string ToString() => Value?.ToString() ?? Resources.Nothing;
}