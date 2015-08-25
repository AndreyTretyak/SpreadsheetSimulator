using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.ExpressionParsers;

namespace Spreadsheet.Core
{
    public interface ISpreadsheet : IEnumerable<ICell>
    {
        int RowCount { get; }

        int ColumnCount { get; }

        ICell this[CellAddress index] { get; }
    }

    public class Spreadsheet : ISpreadsheet
    {
        private readonly Cell[,] _content;

        public Spreadsheet(int rowCount, int columnCount, IEnumerable<Cell> content)
        {
            _content = new Cell[rowCount, columnCount];
            foreach (var cell in content)
            {
                _content[cell.Address.Row, cell.Address.Column] = cell;
            }
        }
        public int RowCount => _content.GetLength(0);

        public int ColumnCount => _content.GetLength(1);

        public ICell this[CellAddress index] => _content[index.Row, index.Column];
        
        public IEnumerator<ICell> GetEnumerator() => _content.Cast<ICell>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    //public class EvaluatedSpreadsheet : ISpreadsheet
    //{
    //    public CellAddress MaxAddress { get; }

    //    private EvaluatedCell[,] _content;

    //    public EvaluatedSpreadsheet(CellAddress maxAddress, IEnumerable<object> values)
    //    {
    //        _content = new EvaluatedCell[maxAddress.Column + 1,maxAddress.Row + 1]; 
    //        var index = 0;
    //        foreach (var value in values)
    //        {
    //            var row = index / maxAddress.Column;
    //            var column = index % maxAddress.Column;
    //            _content[row, column] = new EvaluatedCell(new CellAddress(row, column) , value);
    //            index++;
    //        }
    //    }

    //    public object GetCellValue(CellAddress cellAddress)
    //    {
    //        cellAddress.Validate(cellAddress);
    //        return _content[cellAddress.Row, cellAddress.Column].Evaluate(this);
    //    }

    //    public IEnumerator<ICell> GetEnumerator()
    //    {
    //        return _content.Cast<ICell>().GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }
    //}
}