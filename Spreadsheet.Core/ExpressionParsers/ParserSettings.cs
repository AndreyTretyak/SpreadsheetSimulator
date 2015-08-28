namespace Spreadsheet.Core.ExpressionParsers
{
    public static class ParserSettings
    {
        public const char StringStart = '\'';

        public const char ExpressionStart = '=';

        public const char LeftParanthesis = '(';

        public const char RightParanthesis = ')';

        public const char StreamEndChar = '\0';

        public const char CellSeparatorChar = '\t';

        public const string RowSeparators = "\r\n";

        public const char RowNumberStartLetter = 'A';

        public const int LettersUsedForRowNumber = 26;
    }
}