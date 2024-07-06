using System.Collections.Generic;

namespace Spreadsheet.Core;

public class SpreadsheetProcessingResult(int columnsCount, IEnumerable<object> values)
{
    public int ColumnCount { get; } = columnsCount;

    public IEnumerable<object> Values { get; } = values;
}