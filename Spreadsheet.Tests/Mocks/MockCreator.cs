using System.Linq;
using Spreadsheet.Core;
using Spreadsheet.Core.Cells;

namespace Spreadsheet.Tests.Mocks
{
    internal static class MockCreator
    {
        public static SpreadsheetProcessor CreateProcessor()
        {
            return new SpreadsheetProcessor(new Core.Spreadsheet(0, 0, Enumerable.Empty<Cell>()));
        }
    }
}
