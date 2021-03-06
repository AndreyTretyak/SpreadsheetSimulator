using System.Linq;
using System.Text;
using Spreadsheet.Core.Utils;

namespace Spreadsheet.Core.Cells
{
    public struct CellAddress
    {
        public int Row { get; }

        public int Column { get; }

        public CellAddress(int row, int column)
        {
            Row = row;
            if (row < 0)
                throw new InvalidCellAdressException(Resources.NegetiveCellRow);

            Column = column;
            if (column < 0)
                throw new InvalidCellAdressException(Resources.NegativeCellColumn);
        }

        public void Validate(int maxRow, int maxColumn)
        {
            if (maxRow <= Row)
                throw new InvalidCellAdressException(Resources.WrongCellRow);
            if (maxColumn <= Column)
                throw new InvalidCellAdressException(Resources.WrongCellColumn);
        }

        public bool Equals(CellAddress other)
        {
            return Row == other.Row && Column == other.Column;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is CellAddress && Equals((CellAddress)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Row * 397) ^ Column;
            }
        }

        //Slow method with large memory allocation, done for debug purpose 
        public override string ToString() => CellAddressConverter.GetString(this);
    }
}