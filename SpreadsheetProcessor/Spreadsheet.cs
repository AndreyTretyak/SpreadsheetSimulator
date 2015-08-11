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

        public Cell GetCell(CellAdress cellAdress)
        {
            var value = Source.GetCellContent(cellAdress);

            return null;
        }
    }
}