namespace Spreadsheet.Core.Cells.Expressions
{
    internal class ConstantExpression : IExpression
    {
        internal object Value { get; }

        public ConstantExpression(object value)
        {
            Value = value;
        }

        public object Evaluate(SpreadsheetProcessor processor) => Value;

        public override string ToString() => Value?.ToString() ?? Resources.Nothing;
    }
}