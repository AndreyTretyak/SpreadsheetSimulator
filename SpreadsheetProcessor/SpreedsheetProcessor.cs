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
        private Spreadsheet Spreadsheet { get;  }

        public SpreedsheetProcessor(Spreadsheet spreadsheet)
        {
            Spreadsheet = spreadsheet;
        }

        public ExpressionValue GetCellValue(CellAddress cellAddress)
        {
            return GetCellValue(cellAddress, null);
        }

        internal ExpressionValue GetCellValue(CellAddress cellAddress, string callStack)
        {
            var value = Spreadsheet.GetCell(cellAddress);
            return value.Evaluate(this, callStack);
        }
    }
}
