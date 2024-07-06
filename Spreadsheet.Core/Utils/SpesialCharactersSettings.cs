namespace Spreadsheet.Core.Utils;

internal static class SpesialCharactersSettings
{
    public const char StringStart = '\'';

    public const char ExpressionStart = '=';

    public const char WhiteSpace = ' ';

    public const char CellSeparator = '\t';

    public const char RowSeparator = '\n';

    public const char CarriageReturn = '\r';

    public const char StreamEnd = '\0';

    public const char ColumnStartLetter = 'A';

    public const int LettersUsedForColumnNumber = 26;

    public const char LeftParenthesis = '(';

    public const char RightParathesis = ')';

    public const string ErrorStartMarker = "#";

    public static bool IsColumnLetter(char character)
    {
        character = char.ToUpper(character);
        //We subtract letter because LettersUsedForColumnNumber means 1
        return character >= ColumnStartLetter && character <= ColumnStartLetter + LettersUsedForColumnNumber - 1;
    }

    public static bool IsSeparationCharacter(char character)
    {
        return character == CellSeparator
            || character == CarriageReturn
            || character == RowSeparator
            || character == StreamEnd;
    }
}