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

        public object Evaluate(ISpreadsheet processor, string callStack)
        {
            Address.Validate(processor.MaxAddress);
            return processor.GetCell(Address).Evaluate(processor, callStack);
        }

        public override string ToString() => Address.StringValue;
    }
}