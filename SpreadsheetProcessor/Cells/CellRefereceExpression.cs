namespace SpreadsheetProcessor.Cells
{
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
}