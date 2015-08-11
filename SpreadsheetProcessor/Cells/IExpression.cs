namespace SpreadsheetProcessor.Cells
{
    public interface IExpression
    {
        ExpressionValue Evaluate(SpreedsheetProcessor processor, string callStack);
    }
}