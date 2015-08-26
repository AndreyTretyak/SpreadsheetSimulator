namespace Spreadsheet.Core.Cells
{
    public class ConstantExpression : IExpression
    {
        public object Value { get; }

        public ConstantExpression(object value)
        {
            Value = value;
        }

        public object Evaluate(SpreadsheetProcessor processor) => Value;

        public override string ToString() => Value?.ToString() ?? Resources.Nothing;
    }
}