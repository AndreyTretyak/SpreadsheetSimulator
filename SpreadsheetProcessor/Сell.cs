using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace SpreadsheetProcessor.Cells
{
    [Flags]
    public enum CellValueType
    {
        Error = 1,
        Nothing = 2,
        Integer = 4,
        String = 8
    }
    
    public class Cell
    {
        public CellAdress Adress { get; }

        public IExpression Expression { get; }

        public Cell(CellAdress adress, IExpression expression)
        {
            Adress = adress;
            Expression = expression;
        }

        public ExpressionValue Evaluate(SpreedsheetProcessor processor, string callStack = null)
        {
            if (Expression == null)
                return new ExpressionValue(CellValueType.Nothing, null);

            if (callStack == null)
                callStack = string.Empty;

            if (callStack.Contains(Adress.StringValue))
                return new ExpressionValue(CellValueType.Error, string.Format(Resources.CircularReferenceDetected, Adress.StringValue));
            
            callStack += Adress.StringValue + ParserSettings.CallStackSeparator;

            return Expression.Evaluate(processor, callStack);
        }
    }

    public class ExpressionValue
    {
        public CellValueType Type { get; }

        public object Value { get; }

        public ExpressionValue(CellValueType type, object value)
        {
            Type = type;
            Value = value;
        }

        public string StringRepresentation => Value?.ToString() ?? Resources.Nothing;
    }

    public interface IExpression
    {
        ExpressionValue Evaluate(SpreedsheetProcessor processor, string callStack);
    }

    public class BinaryExpression : IExpression
    {
        public IExpression Left { get; }

        public IExpression Right { get; }

        public string Operator { get; }

        public BinaryExpression(IExpression left, string @operator, IExpression right)
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
    }

    public class CellRefereceExpression : IExpression
    {
        public CellAdress Adress { get; }

        public CellRefereceExpression(CellAdress adress)
        {
            Adress = adress;
        }

        public ExpressionValue Evaluate(SpreedsheetProcessor processor, string callStack)
        {
            return processor.GetCellValue(Adress, callStack);
        }
    }

    public class ConstantExpression : IExpression
    {
        public ExpressionValue Value { get; }

        public ConstantExpression(ExpressionValue value)
        {
            Value = value;
        }

        public ExpressionValue Evaluate(SpreedsheetProcessor processor, string callStack)
        {
            return Value;
        }
    }
}
