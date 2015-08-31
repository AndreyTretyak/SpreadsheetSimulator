﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Spreadsheet.Core;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Utils;

namespace Spreadsheet.Tests
{
    [TestFixture]
    public class CellAdressTests
    {
        [Test]
        [TestCase(5, 1)]
        [TestCase(1, 5)]
        [TestCase(5, 5)]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        [ExpectedException(typeof(InvalidCellAdressException))]
        public void ExceptionTests(int row, int column)
        {
            new CellAddress(row, column).Validate(2, 2);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(2, 0)]
        [TestCase(0, 2)]
        public void ValidateTests(int row, int column)
        {
            new CellAddress(row, column).Validate(3, 3);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(0, 1)]
        [TestCase(123, 174)]
        [TestCase(64, 1981)]
        public void EqualsTest(int row, int column)
        {
            var first = new CellAddress(row, column);
            var second = new CellAddress(row, column);
            Assert.IsFalse(first.Equals(null),"first null equals check");
            Assert.IsFalse(second.Equals(null), "second null equals check");
            Assert.IsTrue(first.Equals(second));
            Assert.IsTrue(second.Equals(first));
            Assert.AreEqual(first.GetHashCode(),second.GetHashCode());
        }
    }
}
