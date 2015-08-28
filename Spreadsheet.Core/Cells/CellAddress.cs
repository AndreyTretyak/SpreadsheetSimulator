using System;
using System.IO;
using System.Linq;
using System.Text;
using Spreadsheet.Core.ExpressionParsers;

namespace Spreadsheet.Core
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

        public CellAddress(string reference)
        {
            var index = 0;
            //transformation char index to  zero based row index
            var column = 0;
            while (index < reference.Length && char.IsLetter(reference[index]))
            {
                column = column * ParserSettings.LettersUsedForRowNumber + (reference[index] - ParserSettings.RowNumberStartLetter + 1);
                index++;
            }
            Column = column - 1;

            //manual number parsing to avoid memory allocation on Substring call
            var row = 0;
            while (index < reference.Length && char.IsDigit(reference[index]))
            {
                row = row * 10 + (reference[index] - '0');
                index++;
            }
            Row = row - 1;

            if (index < reference.Length)
                throw new ExpressionParsingException(string.Format(Resources.WrongCellAddress, reference));
        }

        public void Validate(CellAddress maxPosible)
        {
            string error = null;
            if (Row < 0)
                error += Resources.NegetiveCellRow;
            if (Column < 0)
                error += Resources.NegativeCellColumn;
            if (maxPosible.Row <= Row)
                error += Resources.WrongCellRow;
            if (maxPosible.Column <= Column)
                error += Resources.WrongCellColumn;

            if (!string.IsNullOrEmpty(error))
                throw new InvalidCellAdressException(error);
        }

        private string GetColumnLetter()
        {
            //transformation of zero based row index to char index
            var index = Column + 1;
            var result = new StringBuilder();
            while (index / ParserSettings.LettersUsedForRowNumber > 1)
            {
                result.Append((char) (ParserSettings.RowNumberStartLetter + index % ParserSettings.LettersUsedForRowNumber - 1));
                index = index / ParserSettings.LettersUsedForRowNumber;
            }
            result.Append((char)(ParserSettings.RowNumberStartLetter + index - 1));
            return new string(result.ToString().Reverse().ToArray());
        }

        public string StringValue => $"{GetColumnLetter()}{Row + 1}";

        public override string ToString() => StringValue;

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
    }
}