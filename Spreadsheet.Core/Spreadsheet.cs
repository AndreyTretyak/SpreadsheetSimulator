using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography.X509Certificates;
using SpreadsheetProcessor.Cells;
using SpreadsheetProcessor.ExpressionParsers;

namespace SpreadsheetProcessor
{
    public interface ISpreadsheet
    {
        CellAddress MaxAddress { get; }

        Cell GetCell(CellAddress cellAddress);
    }

    public class Spreadsheet : ISpreadsheet
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

    public class SpreadsheetArray : ISpreadsheet
    {
        public CellAddress MaxAddress { get; }

        private readonly IExpression[,] _content;

        public SpreadsheetArray(CellAddress maxAddress, IEnumerable<Cell> content)
        {
            MaxAddress = maxAddress;
            _content = new IExpression[maxAddress.Row + 1, maxAddress.Column + 1];
            foreach (var cell in content)
            {
                _content[cell.Address.Row, cell.Address.Column] = cell.Expression;
            }
        }

        public Cell GetCell(CellAddress cellAddress)
        {
            var validationResult = cellAddress.Validate(MaxAddress);
            if (!string.IsNullOrWhiteSpace(validationResult))
                throw new SpreadsheatReadingException(validationResult);
            return new Cell(cellAddress, _content[cellAddress.Row,cellAddress.Column]);
        }
    }

    public class SpreadsheetMemoryCache : ISpreadsheet
    {
        public CellAddress MaxAddress { get; }

        private readonly MemoryCache _cache;

        public SpreadsheetMemoryCache(CellAddress maxAddress, IEnumerable<Cell> content)
        {
            MaxAddress = maxAddress;
            _cache = new MemoryCache("Spreadsheet");
            var policy = new CacheItemPolicy();          
            foreach (var cell in content)
            {
                _cache.Add(cell.Address.StringValue, cell.Expression, policy);
            }
        }

        public Cell GetCell(CellAddress cellAddress)
        {
            var validationResult = cellAddress.Validate(MaxAddress);
            if (!string.IsNullOrWhiteSpace(validationResult))
                throw new SpreadsheatReadingException(validationResult);
            return new Cell(cellAddress, (IExpression)_cache.Get(cellAddress.StringValue));
        }
    }
}