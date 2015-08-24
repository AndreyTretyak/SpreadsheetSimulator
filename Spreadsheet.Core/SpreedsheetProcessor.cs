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

        public object GetCellValue(CellAddress cellAddress)
        {
            return GetCellValue(cellAddress, null);
        }

        internal object GetCellValue(CellAddress cellAddress, string callStack)
        {
            try
            {
                return _spreadsheet.GetCell(cellAddress).Evaluate(_spreadsheet, callStack);
            }
            catch (ExpressionEvaluationException ex)
            {
                //TODO: error should be saved as cell value
                return ex;
            }
        }
    }

    public class SpreedsheetProcessorNew
    {
        public void Process(ISpreadsheet spreadsheet)
        {

        }
    }
}
