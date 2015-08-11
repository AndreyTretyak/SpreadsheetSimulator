namespace SpreadsheetProcessor.Cells
{
    public class ConstantExpression : IExpression
    {
        public ExpressionValue Value { get; }

        public ConstantExpression(ExpressionValue value)
        {
            Value = value;
        }

        public ExpressionValue Evaluate(SpreedsheetProcessor processor, string callStack) => Value;
    }
}