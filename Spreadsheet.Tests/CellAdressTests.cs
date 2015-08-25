using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Spreadsheet.Core;
using Spreadsheet.Core.ExpressionParsers;

namespace Spreadsheet.Tests
{
    [TestFixture]
    public class CellAdressTests
    {

        [Test]
        [TestCase("C7")]
        [TestCase("CAD71")]
        [TestCase("Z9801")]
        [TestCase("ZDSR901")]
        [TestCase("BVC197")]
        public void TestStringAddress(string reference)
        {
            Assert.AreEqual(reference, new CellAddress(reference).StringValue);
        }

        [Test]
        [TestCase("A1", 0, 0)]
        [TestCase("A2", 1, 0)]
        [TestCase("B1", 0, 1)]
        [TestCase("BVC197", 196, 1926)]
        public void TestAddress(string reference, int row, int column)
        {
            var address = new CellAddress(reference);
            Assert.AreEqual(row, address.Row, "Wrong row value");
            Assert.AreEqual(column, address.Column, "Wrong column value");
        }

        [Test]
        [TestCase(5, 1, "B2")]
        [TestCase(1, 5, "B2")]
        [TestCase(5, 5, "B2")]
        [TestCase(-1, 1, "B2")]
        [TestCase(1, -1, "B2")]
        [ExpectedException(typeof(InvalidCellAdressException))]
        public void TestErrors(int row, int column, string maxAddress)
        {
            new CellAddress(row, column).Validate(new CellAddress(maxAddress));
        }
    }
}
