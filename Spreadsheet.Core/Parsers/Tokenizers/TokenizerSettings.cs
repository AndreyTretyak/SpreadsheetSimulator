namespace Spreadsheet.Core.Parsers.Tokenizers
{
    public static class TokenizerSettings
    {
        public const char StringStart = '\'';

        public const char ExpressionStart = '=';

        public const char WhiteSpace = ' ';

        public const char CellSeparator = '\t';

        public const char RowSeparator = '\n';

        public const char CarriageReturn = '\r';

        public const char StreamEndChar = '\0';

        public const char RowNumberStartLetter = 'A';

        public const int LettersUsedForRowNumber = 26;

        public const char LeftParanthesis = '(';

        public const char RightParanthesis = ')';
    }
}