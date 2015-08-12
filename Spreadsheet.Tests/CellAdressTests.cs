using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SpreadsheetProcessor;

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
    }
}
