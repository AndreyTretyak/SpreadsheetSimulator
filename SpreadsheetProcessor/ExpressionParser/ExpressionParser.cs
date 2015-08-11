using System;
using System.Linq;
using SpreadsheetProcessor.Cells;

namespace SpreadsheetProcessor.ExpressionParser
{
    public class ExpressionParser
    {
        public IExpression Parse(string expresion)
        {
            if (string.IsNullOrWhiteSpace(expresion))
                return new ConstantExpression(new ExpressionValue(CellValueType.Nothing, null));

            var firstChar = expresion.First();

            if (char.IsDigit(firstChar))
                return ParseNumber(expresion);

            switch (firstChar)
            {
                case ParserSettings.StringStart:
                    return ParseString(expresion);
                case ParserSettings.ExpressionStart:
                    return ParseExpression(expresion);
            }

            return new ConstantExpression(new ExpressionValue(CellValueType.Error, string.Format(Resources.InvalidCellContent, expresion)));
        }

        private IExpression ParseNumber(string expresion)
        {
            int result;
            return int.TryParse(expresion, out result) 
                ? new ConstantExpression(new ExpressionValue(CellValueType.Integer,  result)) 
                : new ConstantExpression(new ExpressionValue(CellValueType.Error, string.Format(Resources.FailedToParseNumber, expresion)));
        }

        private IExpression ParseString(string expresion)
        {
            return new ConstantExpression(new ExpressionValue(CellValueType.String, expresion.Substring(1)));
        }

        private IExpression ParseExpression(string expresion)
        {
            throw new NotImplementedException();
        }
    }
}