namespace SpreadsheetProcessor.Cells
{
    public class ConstantExpression : IExpression
    {
        public object Value { get; }

        public ConstantExpression(object value)
        {
            Value = value;
        }

        public object Evaluate(ISpreadsheetProcessor processor) => Value;

        public override string ToString() => Value?.ToString() ?? Resources.Nothing;
    }
}