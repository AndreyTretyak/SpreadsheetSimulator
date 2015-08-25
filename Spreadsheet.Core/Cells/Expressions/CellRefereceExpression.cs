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

        public object Evaluate(ISpreadsheetProcessor processor)
        {
            //Address.Validate(spreadsheet.MaxAddress);
            return processor.GetCellValue(Address);
        }

        public override string ToString() => Address.StringValue;
    }
}