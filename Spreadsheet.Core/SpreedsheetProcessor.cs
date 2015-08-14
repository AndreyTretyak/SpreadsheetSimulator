using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using SpreadsheetProcessor.Cells;
using SpreadsheetProcessor.ExpressionParsers;

namespace SpreadsheetProcessor
{
    public class SpreedsheetProcessor
    {
        private readonly ISpreadsheet _spreadsheet;

        public CellAddress MaxAddress => _spreadsheet.MaxAddress;

        public SpreedsheetProcessor(ISpreadsheet spreadsheet)
        {
            _spreadsheet = spreadsheet;
        }

        public ExpressionValue GetCellValue(CellAddress cellAddress)
        {
            return GetCellValue(cellAddress, null);
        }

        internal ExpressionValue GetCellValue(CellAddress cellAddress, string callStack)
        {
            try
            {
                return _spreadsheet.GetCell(cellAddress).Evaluate(this, callStack);
            }
            catch (ExpressionEvaluationException ex)
            {
                return new ExpressionValue(CellValueType.Error, ex.Message);
            }
        }
    }
}
