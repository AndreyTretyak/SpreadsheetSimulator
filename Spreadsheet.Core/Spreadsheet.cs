using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using SpreadsheetProcessor.Cells;
using SpreadsheetProcessor.ExpressionParsers;

namespace SpreadsheetProcessor
{
    public interface ISpreadsheet : IEnumerable<ICell>
    {
        CellAddress MaxAddress { get; }

        object GetCellValue(CellAddress cellAddress);
    }

    public class Spreadsheet : ISpreadsheet
    {
        public CellAddress MaxAddress { get; }

        private readonly Cell[,] _content;

        private readonly MemoryCache cache;

        public Spreadsheet(CellAddress maxAddress, IEnumerable<Cell> content)
        {
            MaxAddress = maxAddress;
            _content = new Cell[maxAddress.Row, maxAddress.Column];
            foreach (var cell in content)
            {
                _content[cell.Address.Row, cell.Address.Column] = cell;
            }
            cache = new MemoryCache("Spreadsheet");
        }

        public object GetCellValue(CellAddress cellAddress)
        {
            cellAddress.Validate(MaxAddress);
            //return _content[cellAddress.Row,cellAddress.Column].Evaluate(this);
            var key = cellAddress.StringValue;

            if (cache.Contains(key))
                return cache.GetCacheItem(key);
            var value = new Lazy<object>(() => _content[cellAddress.Row, cellAddress.Column].Evaluate(this), 
                                               LazyThreadSafetyMode.ExecutionAndPublication);
            cache.Add(key, value, DateTimeOffset.MaxValue);
            return value.Value;
        }
        
        public IEnumerator<ICell> GetEnumerator()
        {
            return _content.Cast<ICell>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class EvaluatedSpreadsheet : ISpreadsheet
    {
        public CellAddress MaxAddress { get; }

        private EvaluatedCell[,] _content;

        public EvaluatedSpreadsheet(CellAddress maxAddress, IEnumerable<object> values)
        {
            _content = new EvaluatedCell[maxAddress.Column + 1,maxAddress.Row + 1]; 
            var index = 0;
            foreach (var value in values)
            {
                var row = index / maxAddress.Column;
                var column = index % maxAddress.Column;
                _content[row, column] = new EvaluatedCell(new CellAddress(row, column) , value);
                index++;
            }
        }

        public object GetCellValue(CellAddress cellAddress)
        {
            cellAddress.Validate(cellAddress);
            return _content[cellAddress.Row, cellAddress.Column].Evaluate(this);
        }

        public IEnumerator<ICell> GetEnumerator()
        {
            return _content.Cast<ICell>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}