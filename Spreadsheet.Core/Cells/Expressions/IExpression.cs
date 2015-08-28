using Spreadsheet.Core.ExpressionParsers;

namespace Spreadsheet.Core.Cells
{
    public interface IExpression
    {
        object Evaluate(SpreadsheetProcessor processor);
    }
}