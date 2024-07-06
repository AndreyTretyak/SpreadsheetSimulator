using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Spreadsheet.Core;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Cells.Expressions;
using Spreadsheet.Core.Exceptions;
using Spreadsheet.Core.ProcessingStrategies;
using Spreadsheet.Tests.Mocks;

using static Spreadsheet.Tests.Utils.TestExtensions;

namespace Spreadsheet.Tests;

[TestFixture]
public class SpreadsheetProcessorTests
{
    private static SpreadsheetProcessor CreateProcessor(params IExpression[] expressions)
    {

        return new SpreadsheetProcessor(new Core.Spreadsheet(1, expressions.Length,
            expressions.Select((e, i) => new Cell(new CellAddress(0, i), e))));
    }

    [Test]
    [TestCase(123)]
    [TestCase("test")]
    [TestCase(null)]
    public void EvaluateTest(object value)
    {
        var expression = new ExpressionMock(() => value);
        var processor = CreateProcessor(expression);
        for (var i = 0; i < 5; i++)
        {
            var result = processor.Evaluate(new SimpleProcessingStrategy());
            Assert.AreEqual(value, result.Values.First());
            Assert.AreEqual(processor, expression.Processor);

            //check that value cashed, and expression Evaluate method calls only once.
            Assert.AreEqual(1, expression.EvaluateCallCount);
        }
    }

    [Test]
    [TestCase(321)]
    [TestCase("stake")]
    [TestCase(null)]
    public void UncashedValuesTestTest(object value)
    {
        var expression = new ConstantExpression(value);
        var processor = CreateProcessor(expression);
        var result = processor.Evaluate(new SimpleProcessingStrategy());
        Assert.AreEqual(value, result.Values.First());
    }

    private Exception ExceptionTest(Exception value)
    {
        var expression = new ExpressionMock(() => { throw value; });
        var processor = CreateProcessor(expression);
        var results = processor.Evaluate(new SimpleProcessingStrategy()).Values.ToArray();
        Assert.AreEqual(processor, expression.Processor);
        return Cast<Exception>(results.First());
    }

    [Test]
    public void EvaluaionExceptionTest()
    {
        var exception = new ExpressionEvaluationException("error");
        var result = ExceptionTest(exception);
        Assert.AreEqual(exception, result);
    }


    [Test]
    public void InvalidDataExceptionTest()
    {
        var exception = new InvalidDataException("error");
        var result = ExceptionTest(exception);
        Assert.AreEqual(exception, result.InnerException);
    }

    [Test]
    public void CircularReferenceTest()
    {
        var processor = CreateProcessor(new CellRefereceExpression(new CellAddress(0, 0)));
        Cast<CircularCellRefereceException>(processor.Evaluate(new SimpleProcessingStrategy()).Values.First());
    }

    [Test]
    public void CrossReferenceTest()
    {
        var processor = CreateProcessor(new CellRefereceExpression(new CellAddress(0, 1)),
                                        new CellRefereceExpression(new CellAddress(0, 0)));
        var array = processor.Evaluate(new SimpleProcessingStrategy()).Values.ToArray();
        Cast<CircularCellRefereceException>(array[0]);
        Cast<CircularCellRefereceException>(array[1]);
    }
}
