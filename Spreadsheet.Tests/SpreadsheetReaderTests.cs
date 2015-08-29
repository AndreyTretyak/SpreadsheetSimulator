using System.IO;
using NUnit.Framework;
using Spreadsheet.Core;
using Spreadsheet.Core.Cells.Expressions;
using Spreadsheet.Core.Parsers;

namespace Spreadsheet.Tests
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

    [TestFixture]
    public class SpreadsheetReaderTests
    {
        private Core.Spreadsheet ReadSpreadsheet(string size, IExpression[] expressions)
        {
            using (var reader = new SpreadsheetReader(new StreamReader(StreamUtils.GenerateStreamFromString(size)), 
                                                      s => new SpreadsheetParserMock(expressions)))
            {
                return reader.ReadSpreadsheet();
            }
        }
    }
}
