using NUnit.Framework;

using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Cells.Expressions;
using Spreadsheet.Tests.Mocks;
using Spreadsheet.Tests.Utils;

namespace Spreadsheet.Tests;

[TestFixture]
public class CellTests
{
    [Test]
    [TestCase(123)]
    [TestCase("test")]
    [TestCase(null)]
    public void EvaluateTests(object value)
    {
        var processor = TestExtensions.CreateProcessor();
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
}
