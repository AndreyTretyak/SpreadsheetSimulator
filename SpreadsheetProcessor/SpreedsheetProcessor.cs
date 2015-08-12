using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using SpreadsheetProcessor.Cells;

namespace SpreadsheetProcessor
{
    public class SpreedsheetProcessor
    {
        private readonly Spreadsheet _spreadsheet;

        public SpreedsheetProcessor(Spreadsheet spreadsheet)
        {
            _spreadsheet = spreadsheet;
        }

        public ExpressionValue GetCellValue(CellAddress cellAddress)
        {
            return GetCellValue(cellAddress, null);
        }

        internal ExpressionValue GetCellValue(CellAddress cellAddress, string callStack)
        {
            var value = _spreadsheet.GetCell(cellAddress);
            return value.Evaluate(this, callStack);
        }
    }
}
