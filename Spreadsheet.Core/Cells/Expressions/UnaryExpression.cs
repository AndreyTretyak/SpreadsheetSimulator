using Spreadsheet.Core.ExpressionParsers;

namespace Spreadsheet.Core.Cells
{
    public class UnaryExpression : IExpression
    {
        public IExpression Value { get; }

        public IOperator Operation { get; set; }

        public UnaryExpression(IOperator operation, IExpression value)
        {
            Value = value;
            Operation = operation;
        }

        public object Evaluate(SpreadsheetProcessor processor)
        {
            return Operation.UnaryOperation(Value.Evaluate(processor));
        }

        public override string ToString() => Operation.ToString() + Value;
    }
}