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
        public void TestStringAdress(string reference)
        {
            Assert.AreEqual(reference, new CellAddress(reference).StringValue);
        }
    }
}
