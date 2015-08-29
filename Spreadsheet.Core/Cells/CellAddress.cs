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
            Column = column;
        }

        public void Validate(int maxRow, int maxColumn)
        {
            string error = null;
            if (Row < 0)
                error += Resources.NegetiveCellRow;
            if (Column < 0)
                error += Resources.NegativeCellColumn;
            if (maxRow <= Row)
                error += Resources.WrongCellRow;
            if (maxColumn <= Column)
                error += Resources.WrongCellColumn;

            if (!string.IsNullOrEmpty(error))
                throw new InvalidCellAdressException(error);
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