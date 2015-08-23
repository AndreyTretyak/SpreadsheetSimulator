using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetProcessor.ExpressionParsers
{
    internal class ExpressionParsingException : InvalidOperationException
    {
        public ExpressionParsingException(string message) : base(message)
        {
        }
    }

    internal class ExpressionEvaluationException : InvalidOperationException
    {
        public ExpressionEvaluationException(string message) : base(message)
        {
        }
    }

    internal class SpreadsheatReadingException : InvalidOperationException
    {
        public SpreadsheatReadingException(string message) : base(message)
        {
        }
    }

    internal class InvalidCellAdressException : InvalidOperationException
    {
        public InvalidCellAdressException(string message) : base(message)
        {
        }
    }
}
