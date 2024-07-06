using System;

using Spreadsheet.Core.Cells.Expressions;

namespace Spreadsheet.Core.Cells;

public class Cell
{
    public CellAddress Address { get; }

    public bool IsCashingRequered { get; }

    internal IExpression Expression { get; }

    internal Cell(CellAddress address, IExpression expression)
    {
        Address = address;
        Expression = expression;
        IsCashingRequered = !(Expression is ConstantExpression);
    }

    public object Evaluate(SpreadsheetProcessor processor)
    {
        try
        {
            return Expression.Evaluate(processor);
        }
        catch (SpreadsheetException exception)
        {
            throw SpreadsheetException.AddCellAddressToErrorStack(exception, Address);
        }
        catch (Exception exception)
        {
            throw SpreadsheetException.AddCellAddressToErrorStack(new ExpressionEvaluationException(exception.Message, exception), Address);
        }
    }

    public override string ToString() => $"{Address}|{Expression}";
}
