using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spreadsheet.Core;
using Spreadsheet.Core.Cells.Expressions;

namespace Spreadsheet.Tests.Mocks
{
    public class ExpressionMock : IExpression
    {
        private readonly Func<object> _expression;

        public SpreadsheetProcessor Processor { get; private set; }

        public ExpressionMock(Func<object> expression)
        {
            _expression = expression;
        }

        public object Evaluate(SpreadsheetProcessor processor)
        {
            Processor = processor;
            return _expression();
        }
    }
}
