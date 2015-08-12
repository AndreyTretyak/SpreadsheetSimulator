using SpreadsheetProcessor.Cells;
using SpreadsheetProcessor.ExpressionParsers;

namespace SpreadsheetProcessor
{
    public class Spreadsheet
    {
        private readonly ExpressionParser _parser;

        private readonly SpreadsheetSource _source;

        public CellAddress MaxAddress => _source.MaxAddress;

        public Spreadsheet(SpreadsheetSource source)
        {
            _source = source;
            _parser = new ExpressionParser();
        }

        public Cell GetCell(CellAddress cellAddress)
        {
            IExpression value; 
            try
            {
                value = _parser.Parse(_source.GetCellContent(cellAddress));
                
            }
            catch (ExpressionParsingException ex)
            {
                value = new ConstantExpression(new ExpressionValue(CellValueType.Error, ex.Message));
            }
            return new Cell(cellAddress, value);
        }
    }
}