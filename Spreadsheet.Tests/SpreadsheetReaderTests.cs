using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Spreadsheet.Core;
using Spreadsheet.Core.Cells.Expressions;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Parsers.Operators;
using Spreadsheet.Tests.Mocks;

namespace Spreadsheet.Tests
{
    [TestFixture]
    public class SpreadsheetReaderTests
    {
        private Core.Spreadsheet ReadSpreadsheet(string size, IExpression[] expressions)
        {
            using (var reader = new SpreadsheetReader(new StringReader(size), 
                                                      s => new SpreadsheetParserMock(expressions)))
            {
                return reader.ReadSpreadsheet();
            }
        }

        [Test]
        [ExpectedException(typeof(SpreadsheatReadingException))]
        public void NullTest()
        {
            using (var reader = new SpreadsheetReader((TextReader)null))
            {
                reader.ReadSpreadsheet();
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("12")]
        [TestCase("13a 11")]
        [TestCase("1 17a")]
        [TestCase("-13 16")]
        [TestCase("14 -17")]
        [TestCase("-8 -5")]
        [TestCase("123456789987654321 1")]
        [TestCase("1 123456789987654321")]
        [ExpectedException(typeof(SpreadsheatReadingException))]
        public void WrongSizeTest(string size)
        {
            ReadSpreadsheet(size, new IExpression[0]);
        }

        [Test]
        [TestCase(1, 1)]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        [TestCase(97, 12478)]
        [TestCase(13700, 51)]
        public void SizeTest(int row, int column)
        {
            var spreadsheet = ReadSpreadsheet($"{row} {column}", new IExpression[0]);
            Assert.AreEqual(row, spreadsheet.RowCount, "Wrong row count");
            Assert.AreEqual(column, spreadsheet.ColumnCount, "Wrong column count");
            
            Assert.AreEqual(spreadsheet.RowCount * spreadsheet.ColumnCount, spreadsheet.Count(), "Wrong container size");
        }

        [Test]
        [TestCase(1, 1)]
        [TestCase(1, 6)]
        [TestCase(5, 1)]
        [TestCase(1, 5)]
        [TestCase(2, 2)]
        [TestCase(3, 2)]
        public void ExpressionTest(int row, int column)
        {
            var expressions = new IExpression[]
            {
                new ConstantExpression(0),
                new CellRefereceExpression(new CellAddress(1, 1)),
                new BinaryExpression(new ConstantExpression(1), OperatorManager.Default.Operators['*'],
                new ConstantExpression(2)),
                new UnaryExpression(OperatorManager.Default.Operators['-'], new ConstantExpression(9)),
                new ConstantExpression(91),
                new ConstantExpression("text"),
            };
            var cells = expressions.Select((e, i) => new Cell(new CellAddress(i / column, i % column), e)).ToArray();
            
            var spreadsheet = ReadSpreadsheet($"{row} {column}", expressions);

            Assert.AreEqual(row, spreadsheet.RowCount, "Wrong row count");
            Assert.AreEqual(column, spreadsheet.ColumnCount, "Wrong column count");

            var array = spreadsheet.ToArray();
            Assert.AreEqual(row * column, array.Length, "Wrong container size");

            CollectionAssert.AreEqual(cells.Take(array.Length), 
                                     array, 
                                     new GenericComparer<Cell>((x,y) => string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal)));
        }


        [Test]
        public void ExсeptionTest()
        {
            var exception = new ExpressionParsingException("error");
            using (var reader = new SpreadsheetReader(new StringReader("12 6"),
                                          s => new SpreadsheeParsertWithExceptionMock(exception)))
            {
                Assert.AreEqual(exception, reader.ReadSpreadsheet().First().Evaluate(null));
            }
        }
    }
}
