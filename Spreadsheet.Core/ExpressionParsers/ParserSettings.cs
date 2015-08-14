namespace SpreadsheetProcessors
{
    public static class ParserSettings
    {
        public const char StringStart = '\'';

        public const char ExpressionStart = '=';

        public const char AdditionOperator = '+';

        public const char SubtractionOperator = '-';

        public const char MultiplicationOperator = '*';

        public const char DivisionOperator = '/';

        public const char LeftParanthesis = '(';

        public const char RightParanthesis = ')';

        public static char[] Operators => new [] {AdditionOperator, SubtractionOperator, MultiplicationOperator, DivisionOperator};

        public const string CallStackSeparator = "|";

        public static char ExpressionEndChar = '\0';
    }
}