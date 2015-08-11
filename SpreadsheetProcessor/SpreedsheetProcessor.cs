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

        public ExpressionValue GetCellValue(CellAdress cellAdress)
        {
            return GetCellValue(cellAdress, null);
        }

        internal ExpressionValue GetCellValue(CellAdress cellAdress, string callStack)
        {
            var value = Spreadsheet.GetCell(cellAdress);
            return value.Evaluate(this, callStack);
        }
    }
}
