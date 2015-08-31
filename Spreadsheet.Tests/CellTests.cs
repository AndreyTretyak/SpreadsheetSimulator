using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Spreadsheet.Core;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Cells.Expressions;
using Spreadsheet.Tests.Mocks;
using Spreadsheet = Spreadsheet.Core.Spreadsheet;

namespace Spreadsheet.Tests
{
    [TestFixture]
    public class CellTests
    {
        private static SpreadsheetProcessor CreateProcessor()
        {
            return new SpreadsheetProcessor(new Core.Spreadsheet(0, 0, Enumerable.Empty<Cell>()));
        }

        [Test]
        [TestCase(123)]
        [TestCase("test")]
        [TestCase(null)]
        public void EvaluateTests(object value)
        {
            var processor = CreateProcessor();
            var expression = new ExpressionMock(() => value);
            var cell = new Cell(default(CellAddress), expression);

            Assert.AreEqual(value, cell.Evaluate(processor));
            Assert.AreEqual(processor, expression.Processor);
        }

        [Test]
        public void IsCashingRequeredTest()
        {
            var address = default(CellAddress);

            var cell = new Cell(address, new ConstantExpression(null));
            Assert.IsFalse(cell.IsCashingRequered, nameof(ConstantExpression));

            cell = new Cell(address, new BinaryExpression(null, null, null));
            Assert.IsTrue(cell.IsCashingRequered, nameof(BinaryExpression));

            cell = new Cell(address, new UnaryExpression(null, null));
            Assert.IsTrue(cell.IsCashingRequered, nameof(UnaryExpression));

            cell = new Cell(address, new CellRefereceExpression(address));
            Assert.IsTrue(cell.IsCashingRequered, nameof(CellRefereceExpression));
        }

        [Test]
        public void AddressTest()
        {
            var address = new CellAddress(0, 0);
            var cell = new Cell(address, null);
            Assert.AreEqual(address, cell.Address);
        }
    }
}
