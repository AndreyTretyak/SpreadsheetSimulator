using System;
using Spreadsheet.Core.Cells.Expressions;
using Spreadsheet.Core.Parsers;

namespace Spreadsheet.Tests.Mocks
{
    internal class SpreadsheetParserMock : ISpreadsheetParser
    {
        private int _index;

        private readonly IExpression[] _expressions;

        public SpreadsheetParserMock(IExpression[]  expressions)
        {
            _index = 0;
            _expressions = expressions;
        }

        public IExpression NextExpression()
        {
            return _index < _expressions.Length 
                ? _expressions[_index++] 
                : null;
        }
    }

    internal class SpreadsheeParsertWithExceptionMock : ISpreadsheetParser
    {
        private readonly Exception _exception;

        public SpreadsheeParsertWithExceptionMock(Exception exception)
        {
            _exception = exception;
        }

        public IExpression NextExpression()
        {
            throw _exception;
        }
    }

}