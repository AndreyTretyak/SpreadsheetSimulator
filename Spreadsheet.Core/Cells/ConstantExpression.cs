namespace SpreadsheetProcessor.Cells
{
    public class ConstantExpression : IExpression
    {
        public ExpressionValue Value { get; }

        public ConstantExpression(ExpressionValue value)
        {
            Value = value;
        }

        public ConstantExpression(CellValueType type, object value)
        {
            Value = new ExpressionValue(type, value);
        }

        public ExpressionValue Evaluate(SpreedsheetProcessor processor, string callStack) => Value;

        public override string ToString() => Value.StringRepresentation;
    }
}