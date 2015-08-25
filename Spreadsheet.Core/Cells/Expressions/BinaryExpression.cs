using System.Data;
using Spreadsheet.Core.ExpressionParsers;

namespace Spreadsheet.Core.Cells
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

        public object Evaluate(ISpreadsheetProcessor processor)
        {
            if (Right == null)
                return Left.Evaluate(processor);

            var leftResult = Left.Evaluate(processor) as int?;
            //if (leftResult.Type == CellValueType.Error)
            //    return leftResult;

            var rightResult = Right.Evaluate(processor) as int?;
            //if (rightResult.Type == CellValueType.Error)
            //    return rightResult;
            

            if (!leftResult.HasValue || !rightResult.HasValue)
                throw new ExpressionEvaluationException(Resources.WrongTypeError);

            int? result;
            switch (Operation)
            {
                case ParserSettings.AdditionOperator:
                    result = leftResult.Value + rightResult.Value;
                    break;
                case ParserSettings.SubtractionOperator:
                    result = leftResult.Value - rightResult.Value;
                    break;
                case ParserSettings.MultiplicationOperator:
                    result = leftResult.Value * rightResult.Value;
                    break;
                case ParserSettings.DivisionOperator:
                    if (rightResult.Value == 0)
                        return new ExpressionEvaluationException(Resources.ZeroDivision);
                    result = leftResult.Value / rightResult.Value;
                    break;
                default:
                    throw new ExpressionEvaluationException(string.Format(Resources.UnknownOperator, Operation));
            }
            return result;
        }

        public override string ToString() => Right == null ? Left.ToString() : (ParserSettings.LeftParanthesis + Left + Operation + Right + ParserSettings.RightParanthesis);
    }
}