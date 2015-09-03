using System.Collections.Generic;

namespace Spreadsheet.Core
{
    public class SpreadsheetProcessingResult
    {
        public int ColumnCount { get; }

        public IEnumerable<object> Values { get; }
        
        public SpreadsheetProcessingResult(int columnsCount, IEnumerable<object> values)
        {
            ColumnCount = columnsCount;
            Values = values;
        }
    }
}