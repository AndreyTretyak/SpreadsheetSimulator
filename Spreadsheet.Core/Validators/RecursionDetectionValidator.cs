using System.Collections.Generic;
using Microsoft.CodeAnalysis.Collections;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Cells.Expressions;
using Spreadsheet.Core.Exceptions;

namespace Spreadsheet.Core.Validators;

public class RecursionDetectionValidator : ISpreadsheetValidator
{

    public void Validate(Spreadsheet spreadsheet, Cell cell)
    {
        var hashset = PooledHashSet<CellAddress>.GetInstance();
        try
        {
            CheckRecursion(spreadsheet, cell, hashset);
        }
        finally
        {
            hashset.Free();
        }
    }

    private void CheckRecursion(Spreadsheet spreadsheet, Cell current, ISet<CellAddress> stack)
    {
        try
        {
            var dependencies = PooledHashSet<CellAddress>.GetInstance();
            try
            {
                GetDependencies(current.Expression, dependencies);
                if (dependencies.Overlaps(stack))
                    throw new CircularCellRefereceException(Resources.CircularReference);

                stack.Add(current.Address);
                foreach (var address in dependencies)
                {
                    CheckRecursion(spreadsheet, spreadsheet[address], stack);
                }
                stack.Remove(current.Address);
            }
            finally
            {
                dependencies.Free();
            }
        }
        catch (CircularCellRefereceException ex)
        {
            throw SpreadsheetException.AddCellAddressToErrorStack(ex, current.Address);
        }
    }

    private void GetDependencies(IExpression expression, ISet<CellAddress> addresses)
    {
        var binaryExpression = expression as BinaryExpression;
        if (binaryExpression != null)
        {
            GetDependencies(binaryExpression.Left, addresses);
            GetDependencies(binaryExpression.Right, addresses);
        }

        var unaryExpression = expression as UnaryExpression;
        if (unaryExpression != null)
        {
            GetDependencies(unaryExpression.Value, addresses);
        }

        var refereceExpression = expression as CellRefereceExpression;
        if (refereceExpression != null)
        {
            addresses.Add(refereceExpression.Address);
        }
    }
}