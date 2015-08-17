using SpreadsheetProcessors;

namespace SpreadsheetProcessor.Cells
{
    public class BinaryExpression : IExpression
    {
        public IExpression Left { get; }

        public IExpression Right { get; set; }

        public string Operation { get; set; }

        public BinaryExpression(IExpression left)
        {
            Left = left;
        }

        public BinaryExpression(IExpression left, string operation, IExpression right)
        {
            Left = left;
            Right = right;
            Operation = operation;
        }

        public ExpressionValue Evaluate(ISpreadsheet processor, string callStack)
        {
            if (Right == null)
                return Left.Evaluate(processor, callStack);

            var leftResult = Left.Evaluate(processor, callStack);
            if (leftResult.Type == CellValueType.Error)
                return leftResult;

            var rightResult = Right.Evaluate(processor, callStack);
            if (rightResult.Type == CellValueType.Error)
                return rightResult;
            

            if (leftResult.Type != CellValueType.Integer || rightResult.Type != CellValueType.Integer)
                return new ExpressionValue(CellValueType.Error, Resources.WrongTypeError);

            int result;
            switch (Operation)
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
                    return new ExpressionValue(CellValueType.Error, string.Format(Resources.UnknownOperator, Operation));
            }
            return new ExpressionValue(CellValueType.Integer, result);
        }

        public override string ToString() => Right == null ? Left.ToString() : (ParserSettings.LeftParanthesis + Left + Operation + Right + ParserSettings.RightParanthesis);
    }
}