using Spreadsheet.Core.Cells.Expressions;

namespace Spreadsheet.Core.Parsers;

internal interface ISpreadsheetParser
{
    IExpression ReadExpression();
}