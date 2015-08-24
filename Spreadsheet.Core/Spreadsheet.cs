using System;
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

        IEnumerable<Cell> GetCells();
    }

    public class SpreadsheetArray : ISpreadsheet
    {
        public CellAddress MaxAddress { get; }

        private readonly Cell[,] _content;

        public SpreadsheetArray(CellAddress maxAddress, IEnumerable<Cell> content)
        {
            MaxAddress = maxAddress;
            _content = new Cell[maxAddress.Row, maxAddress.Column];
            foreach (var cell in content)
            {
                _content[cell.Address.Row, cell.Address.Column] = cell;
            }
        }

        public Cell GetCell(CellAddress cellAddress)
        {
            cellAddress.Validate(MaxAddress);
            return _content[cellAddress.Row,cellAddress.Column];
        }

        public IEnumerable<Cell> GetCells()
        {
            return _content.Cast<Cell>();
        }
    }

    /*
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
                _cache.Add(cell.Address.StringValue, cell, policy);
            }
        }

        public Cell GetCell(CellAddress cellAddress)
        {
            var validationResult = cellAddress.Validate(MaxAddress);
            if (!string.IsNullOrWhiteSpace(validationResult))
                throw new SpreadsheatReadingException(validationResult);
            return (Cell)_cache.Get(cellAddress.StringValue);
        }
    }
    */
}