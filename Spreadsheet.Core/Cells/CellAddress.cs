using Spreadsheet.Core.Utils;

namespace Spreadsheet.Core.Cells;

public struct CellAddress
{
    public int Row { get; }

    public int Column { get; }

    public CellAddress(int row, int column)
    {
        Row = row;
        if (row < 0)
            throw new InvalidCellAddressException(Resources.NegetiveCellRow);

        Column = column;
        if (column < 0)
            throw new InvalidCellAddressException(Resources.NegativeCellColumn);
    }

    public readonly void Validate(int maxRow, int maxColumn)
    {
        if (maxRow <= Row)
            throw new InvalidCellAddressException(Resources.WrongCellRow);
        if (maxColumn <= Column)
            throw new InvalidCellAddressException(Resources.WrongCellColumn);
    }

    public bool Equals(CellAddress other) => Row == other.Row && Column == other.Column;

    public override bool Equals(object obj) => obj is not null && obj is CellAddress address && Equals(address);

    public override readonly int GetHashCode()
    {
        unchecked
        {
            return (Row * 397) ^ Column;
        }
    }

    //Slow method with large memory allocation, done for debug purpose 
    public override string ToString() => CellAddressConverter.GetString(this);
}