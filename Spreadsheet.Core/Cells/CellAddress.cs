using System;
using System.IO;
using System.Linq;

namespace SpreadsheetProcessor
{
    public struct CellAddress
    {
        public int Row { get; }

        public int Column { get; }

        public string StringValue => $"{GeColumnLetter()}{Row + 1}";

        public CellAddress(string reference) : this(GetRowNumber(reference),GetColumnNumber(reference))
        {
        }

        public CellAddress(int row, int column)
        {
            if (row < 0)
                throw new IndexOutOfRangeException("Cell row can`t be negative");
            if (column < 0)
                throw new IndexOutOfRangeException("Cell column can`t be negative");
            Row = row;
            Column = column;
        }

        public void Validate(CellAddress maxPosible)
        {
            if (maxPosible.Row <= Row)
                throw new IndexOutOfRangeException("Cell row is more then table size");
            if (maxPosible.Column <= Column)
                throw new IndexOutOfRangeException("Cell column is more then table size");
        }

        private const int LettersUsedForRowNumber = 26;

        private const char StartLetter = 'A';

        private static int GetColumnNumber(string reference)
        {
            //transformation char index to  zero based row index
            return reference.TakeWhile(char.IsLetter)
                            .Reverse()
                            .Select((c, i) => (char.ToUpper(c) - StartLetter + 1) * (int)Math.Pow(LettersUsedForRowNumber, i))
                            .Sum() - 1;
        }

        private string GeColumnLetter()
        {
            //transformation of zero based row index to char index
            var index = Column + 1;
            var result = string.Empty;
            while (index / LettersUsedForRowNumber > 1)
            {
                result = ((char)(StartLetter + index % LettersUsedForRowNumber - 1)) + result;
                index = index / LettersUsedForRowNumber;
            }
            result = ((char)(StartLetter + index - 1)) + result;
            return result;
        }

        private static int GetRowNumber(string reference)
        {
            int result;
            if (int.TryParse(new string(reference.SkipWhile(char.IsLetter).ToArray()), out result))
                return result - 1;
            throw new InvalidDataException($"Unable to parse cell address '{reference}'");
        }
    }
}