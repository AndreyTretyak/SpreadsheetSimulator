using System;

namespace Spreadsheet.Core
{
    internal class ExpressionEvaluationException : SpreadsheetException
    {
        public ExpressionEvaluationException(string message) : base(message) { }

        public ExpressionEvaluationException(string message, Exception innerException) : base(message, innerException) { }
    }
}