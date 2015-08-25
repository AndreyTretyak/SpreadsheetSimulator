namespace Spreadsheet.Core.ExpressionParsers
{
    public static class ParserSettings
    {
        public const string StringStart = "\'";

        public const string ExpressionStart = "=";

        public const string AdditionOperator = "+";

        public const string SubtractionOperator = "-";

        public const string MultiplicationOperator = "*";

        public const string DivisionOperator = "/";

        public const string LeftParanthesis = "(";

        public const string RightParanthesis = ")";

        public const string CallStackSeparator = "|";

        public static char StreamEndChar = '\0';
    }
}