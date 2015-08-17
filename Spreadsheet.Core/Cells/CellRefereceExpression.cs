namespace SpreadsheetProcessor.Cells
{
    public class CellRefereceExpression : IExpression
    {
        public CellAddress Address { get; }

        public CellRefereceExpression(CellAddress address)
        {
            Address = address;
        }

        public ExpressionValue Evaluate(ISpreadsheet processor, string callStack)
        {
            var validationResult = Address.Validate(processor.MaxAddress);
            return string.IsNullOrWhiteSpace(validationResult) 
                   ? processor.GetCell(Address).Evaluate(processor, callStack)
                   : new ExpressionValue(CellValueType.Error, validationResult);
        }

        public override string ToString() => Address.StringValue;
    }
}