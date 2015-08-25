using System;
using System.IO;
using System.Linq;
using System.Text;
using Spreadsheet.Core.ExpressionParsers;

namespace Spreadsheet.Core
{
    public struct CellAddress
    {
        private const char StartLetter = 'A';

        private const int LettersUsedForRowNumber = 26;

        public int Row { get; }

        public int Column { get; }

        public CellAddress(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public CellAddress(string reference) : this(GetRowNumber(reference),GetColumnNumber(reference))
        {
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

        private static int GetRowNumber(string reference)
        {
            int result;
            if (int.TryParse(new string(reference.SkipWhile(char.IsLetter).ToArray()), out result))
                return result - 1;
            throw new ExpressionParsingException($"Unable to parse cell address '{reference}'");
        }

        private static int GetColumnNumber(string reference)
        {
            //transformation char index to  zero based row index
            return reference.TakeWhile(char.IsLetter)
                            .Reverse()
                            .Select((c, i) => (char.ToUpper(c) - StartLetter + 1) * (int)Math.Pow(LettersUsedForRowNumber, i))
                            .Sum() - 1;
        }

        private string GetColumnLetter()
        {
            //transformation of zero based row index to char index
            var index = Column + 1;
            var result = new StringBuilder();
            while (index / LettersUsedForRowNumber > 1)
            {
                result.Append((char) (StartLetter + index%LettersUsedForRowNumber - 1));
                index = index / LettersUsedForRowNumber;
            }
            result.Append((char)(StartLetter + index - 1));
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