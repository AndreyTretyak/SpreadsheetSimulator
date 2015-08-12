using SpreadsheetProcessor.Cells;

namespace SpreadsheetProcessor
{
    public class Spreadsheet
    {
        
        private SpreadsheetSource Source { get; }

        public Spreadsheet(SpreadsheetSource source)
        {
            Source = source;
        }

        public Cell GetCell(CellAddress cellAddress)
        {
            var value = Source.GetCellContent(cellAddress);

            return null;
        }
    }
}