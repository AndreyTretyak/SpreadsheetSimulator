using System;

namespace SpreadsheetProcessor.Cells
{
    public class CellRefereceExpression : IExpression
    {
        public CellAddress Address { get; }

        public CellRefereceExpression(CellAddress address)
        {
            Address = address;
        }

        public object Evaluate(ISpreadsheet spreadsheet)
        {
            Address.Validate(spreadsheet.MaxAddress);
            return spreadsheet.GetCellValue(Address);
        }

        public override string ToString() => Address.StringValue;
    }
}