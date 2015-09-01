using System.Collections.Generic;

namespace Spreadsheet.Core.Cells.Expressions
{
    internal interface IExpression
    {
        object Evaluate(SpreadsheetProcessor processor);
    }
}