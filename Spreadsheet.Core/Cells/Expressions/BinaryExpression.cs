using System.Data;
using Spreadsheet.Core.ExpressionParsers;

namespace Spreadsheet.Core.Cells
{
    public class BinaryExpression : IExpression
    {
        public IExpression Left { get; }

        public IExpression Right { get; }

        public IOperator Operation { get; }

        public BinaryExpression(IExpression left)
        {
            Left = left;
        }

        public BinaryExpression(IExpression left, IOperator operation, IExpression right)
        {
            Left = left;
            Right = right;
            Operation = operation;
        }

        public object Evaluate(SpreadsheetProcessor processor)
        {
            return Right == null 
                   ? Left.Evaluate(processor) 
                   : Operation.BinaryOperation(Left.Evaluate(processor), Right.Evaluate(processor));
        }

        public override string ToString() => Right == null ? Left.ToString() : (ParserSettings.LeftParanthesis + Left.ToString() + Operation + Right + ParserSettings.RightParanthesis);
    }
}