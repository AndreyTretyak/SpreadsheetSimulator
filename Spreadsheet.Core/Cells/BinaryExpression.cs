using SpreadsheetProcessors;

namespace SpreadsheetProcessor.Cells
{
    public class BinaryExpression : IExpression
    {
        public IExpression Left { get; }

        public IExpression Right { get; }

        public char Operator { get; }

        public BinaryExpression(IExpression left, char @operator, IExpression right)
        {
            Left = left;
            Right = right;
            Operator = @operator;
        }

        public ExpressionValue Evaluate(SpreedsheetProcessor processor, string callStack)
        {
            if (Right == null)
                return Left.Evaluate(processor, callStack);

            var leftResult = Left.Evaluate(processor, callStack);
            if (leftResult.Type == CellValueType.Error)
                return leftResult;

            var rightResult = Right.Evaluate(processor, callStack);
            if (rightResult.Type == CellValueType.Error)
                return rightResult;

            //TODO need type validation
            int result;
            switch (Operator)
            {
                case ParserSettings.AdditionOperator:
                    result = (int)leftResult.Value + (int)rightResult.Value;
                    break;
                case ParserSettings.SubtractionOperator:
                    result = (int)leftResult.Value - (int)rightResult.Value;
                    break;
                case ParserSettings.MultiplicationOperator:
                    result = (int)leftResult.Value * (int)rightResult.Value;
                    break;
                case ParserSettings.DivisionOperator:
                    var rightValue = (int) rightResult.Value;
                    if (rightValue == 0)
                        return new ExpressionValue(CellValueType.Error, Resources.ZeroDivision);
                    result = (int)leftResult.Value / rightValue;
                    break;
                default:
                    return new ExpressionValue(CellValueType.Error, string.Format(Resources.UnknownOperator, Operator));
            }
            return new ExpressionValue(CellValueType.Integer, result);
        }

        public override string ToString() => Right == null ? Left.ToString() : Left.ToString() + Operator + Right;
    }
}