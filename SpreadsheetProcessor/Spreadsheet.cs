using SpreadsheetProcessor.Cells;
using SpreadsheetProcessor.ExpressionParsers;

namespace SpreadsheetProcessor
{
    public class Spreadsheet
    {
        private readonly ExpressionParser _parser;

        private readonly SpreadsheetSource _source;

        public Spreadsheet(SpreadsheetSource source)
        {
            _source = source;
            _parser = new ExpressionParser();
        }

        public Cell GetCell(CellAddress cellAddress)
        {
            return new Cell(cellAddress, _parser.Parse(_source.GetCellContent(cellAddress)));
        }
    }
}