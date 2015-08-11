using System;

namespace SpreadsheetProcessor
{
    public struct CellAdress
    {
        public int Row { get; }

        public int Column { get; }

        public string StringValue => $"R{Row}C{Column}";

        public CellAdress(int row, int column)
        {
            if (row < 0)
                throw new IndexOutOfRangeException("Cell row can`t be negative");
            if (column < 0)
                throw new IndexOutOfRangeException("Cell column can`t be negative");
            Row = row;
            Column = column;
        }

        public void Validate(CellAdress maxPosible)
        {
            if (maxPosible.Row > Row)
                throw new IndexOutOfRangeException("Cell row is more then table size");
            if (maxPosible.Column > Column)
                throw new IndexOutOfRangeException("Cell column is more then table size");
        }
    }
}