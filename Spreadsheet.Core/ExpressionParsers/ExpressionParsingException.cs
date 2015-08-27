using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spreadsheet.Core.ExpressionParsers
{
    internal class SpreadsheetException : SystemException
    {
        public SpreadsheetException(string message) : base(message) { }

        public SpreadsheetException(string message, Exception innerException) : base(message, innerException) { }

        public override string ToString() => Message;
    }

    internal class SpreadsheatReadingException : SpreadsheetException
    {
        public SpreadsheatReadingException(string message) : base(message) { }
    }

    internal class ExpressionParsingException : SpreadsheetException
    {
        public ExpressionParsingException(string message) : base(message) { }
    }

    internal class ExpressionEvaluationException : SpreadsheetException
    {
        public ExpressionEvaluationException(string message) : base(message) { }

        public ExpressionEvaluationException(string message, Exception innerException) : base(message, innerException) { }
    }

    internal class InvalidCellAdressException : SpreadsheetException
    {
        public InvalidCellAdressException(string message) : base(message) { }
    }
}
