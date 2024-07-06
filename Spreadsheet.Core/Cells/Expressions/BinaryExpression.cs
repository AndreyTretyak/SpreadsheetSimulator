using Spreadsheet.Core.Parsers.Operators;

using static Spreadsheet.Core.Utils.SpesialCharactersSettings;

namespace Spreadsheet.Core.Cells.Expressions;

internal class BinaryExpression(IExpression left, IOperator operation, IExpression right) : IExpression
{
    public IExpression Left { get; } = left;

    public IExpression Right { get; } = right;

    public IOperator Operation { get; } = operation;

    public object Evaluate(SpreadsheetProcessor processor)
    {
        return Operation.BinaryOperation(Left.Evaluate(processor), Right.Evaluate(processor));
    }

    public override string ToString() => $"{LeftParenthesis}{Left}{Operation}{Right}{RightParathesis}";
}