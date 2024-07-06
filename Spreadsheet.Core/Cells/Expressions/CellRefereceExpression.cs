using System;

namespace Spreadsheet.Core.Cells.Expressions;

internal class CellReferenceExpression(CellAddress address) : IExpression
{
    public CellAddress Address { get; } = address;

    public object Evaluate(SpreadsheetProcessor processor)
    {
        var result = processor.GetCellValue(Address);
        return result is Exception exception ? throw exception : result;
    }

    public override string ToString() => Address.ToString();
}