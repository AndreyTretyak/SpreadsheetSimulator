using System.Linq;
using System.Text;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Parsers.Tokenizers;

namespace Spreadsheet.Core.Utils
{
    internal static class CellAddressConverter
    {
        public static CellAddress FromString(string address)
        {
            var index = 0;
            //transformation char index to  zero based row index
            var column = 0;
            while (index < address.Length && char.IsLetter(address[index]))
            {
                if ((uint)column > (int.MaxValue / ConstantsSettings.LettersUsedForColumnNumber)) //check if next iteration make it bigger that MaxInt
                {
                    throw new InvalidCellAdressException(Resources.IntegerToBig);
                }
                column = column * ConstantsSettings.LettersUsedForColumnNumber + (char.ToUpper(address[index]) - ConstantsSettings.ColumnStartLetter + 1);
                index++;
            }
            column = column - 1;

            //manual number parsing to avoid memory allocation on Substring call
            var row = 0;
            while (index < address.Length && char.IsDigit(address[index]))
            {
                if ((uint)row > (int.MaxValue / 10)) //check if next iteration make it bigger that MaxInt
                {
                    throw new InvalidCellAdressException((Resources.IntegerToBig));
                }
                row = row * 10 + (address[index] - '0');
                index++;
            }
            row = row - 1;

            if (index < address.Length)
                throw new InvalidCellAdressException(string.Format(Resources.WrongCellAddress, address));

            return new CellAddress(row, column);
        }

        public static string GetString(CellAddress address)
        {
            //transformation of zero based row index to char index
            var index = address.Column + 1;
            var result = new StringBuilder();
            while (index / ConstantsSettings.LettersUsedForColumnNumber > 1)
            {
                result.Append((char)(ConstantsSettings.ColumnStartLetter + index % ConstantsSettings.LettersUsedForColumnNumber - 1));
                index = index / ConstantsSettings.LettersUsedForColumnNumber;
            }
            result.Append((char)(ConstantsSettings.ColumnStartLetter + index - 1));
            var column = new string(result.ToString().Reverse().ToArray());

            return $"{column}{address.Row + 1}";
        }
    }

}