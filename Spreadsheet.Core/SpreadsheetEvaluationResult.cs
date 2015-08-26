using System.Collections.Generic;

namespace Spreadsheet.Core
{
    public class SpreadsheetEvaluationResult
    {
        public int ColumnCount { get; }

        public IEnumerable<object> Values { get; }
        
        public SpreadsheetEvaluationResult(int columnsCount, IEnumerable<object> values)
        {
            ColumnCount = columnsCount;
            Values = values;
        }
    }
}