using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Spreadsheet.Core.Cells;

namespace Spreadsheet.Core;

public class Spreadsheet : IEnumerable<Cell>
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

    public Cell this[CellAddress address]
    {
        get
        {
            address.Validate(RowCount, ColumnCount);
            return _content[address.Row, address.Column];
        }
    }

    public IEnumerator<Cell> GetEnumerator() => _content.Cast<Cell>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}