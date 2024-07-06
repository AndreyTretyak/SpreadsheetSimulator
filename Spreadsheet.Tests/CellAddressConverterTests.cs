using NUnit.Framework;
using Spreadsheet.Core;
using Spreadsheet.Core.Utils;

namespace Spreadsheet.Tests;

[TestFixture]
public class CellAddressConverterTests
{
    [Test]
    [TestCase("C7")]
    [TestCase("CAD71")]
    [TestCase("Z9801")]
    [TestCase("ZdrR901")]
    [TestCase("BVC197")]
    public void StringTests(string reference)
    {
        Assert.AreEqual(reference.ToUpper(), CellAddressConverter.FromString(reference).ToString());
    }

    [Test]
    [TestCase("A1", 0, 0)]
    [TestCase("a2", 1, 0)]
    [TestCase("B1", 0, 1)]
    [TestCase("BvC197", 196, 1926)]
    public void AddressTests(string reference, int row, int column)
    {
        var address = CellAddressConverter.FromString(reference);
        Assert.AreEqual(row, address.Row, "Wrong row value");
        Assert.AreEqual(column, address.Column, "Wrong column value");
    }

    [Test]
    [TestCase("A11231232135432543341324")]
    [TestCase("ABSDFDERWEDSASDSDW4")]
    [TestCase("F14@")]
    public void ExceptionTests(string reference)
    {
        Assert.That(() => CellAddressConverter.FromString(reference), Throws.InstanceOf<InvalidCellAddressException>());
    }
}