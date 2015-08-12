namespace SpreadsheetProcessor.Cells
{
    public class CellRefereceExpression : IExpression
    {
        public CellAddress Address { get; }

        public CellRefereceExpression(CellAddress address)
        {
            Address = address;
        }

        public ExpressionValue Evaluate(SpreedsheetProcessor processor, string callStack)
        {
            return processor.GetCellValue(Address, callStack);
        }
    }
}