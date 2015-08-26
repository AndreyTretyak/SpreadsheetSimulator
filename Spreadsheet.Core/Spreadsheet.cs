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
}