using Spreadsheet.Core.Parsers.Operators;

namespace Spreadsheet.Core.Parsers.Tokenizers;

internal interface ISpreadsheetTokenizer
{
    Token Peek();

    Token Read();

    OperatorManager OperatorManager { get; }
}