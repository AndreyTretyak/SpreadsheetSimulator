using System;

namespace Spreadsheet.Core;

public class ExpressionEvaluationException : SpreadsheetException
{
    public ExpressionEvaluationException(string message) : base(message)
    {
    }

    public ExpressionEvaluationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}