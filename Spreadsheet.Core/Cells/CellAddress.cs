using System;
using System.IO;
using System.Linq;
using SpreadsheetProcessor.ExpressionParsers;

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
            Row = row;
            Column = column;
        }

        public string Validate(CellAddress maxPosible)
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
            return error;
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
            throw new ExpressionParsingException($"Unable to parse cell address '{reference}'");
        }

        public override string ToString() => StringValue;
    }
}