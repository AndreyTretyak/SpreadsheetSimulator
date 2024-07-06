using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Spreadsheet.Core;
using Spreadsheet.Core.Cells;
using Spreadsheet.Tests.Mocks;
using Spreadsheet.Core.Cells.Expressions;
using Spreadsheet.Tests.Utils;
using ConstantExpression = Spreadsheet.Core.Cells.Expressions.ConstantExpression;

namespace Spreadsheet.Tests
{
    [TestFixture]
    public class ExpressionTests
    {
        [Test]
        [TestCase(1, 2, 3)]
        [TestCase("1", "2", "3")]
        [TestCase("1", 2, "3")]
        public void BinaryExpressionTest(object left, object right, object result)
        {
            var leftExpression = new ExpressionMock(() => left);
            var rightExpr = new ExpressionMock(() => right);
            var @operator = new OperatorMock(() => result);
            var expression = new Core.Cells.Expressions.BinaryExpression(leftExpression, @operator, rightExpr);

            var processor = TestExtensions.CreateProcessor();
            Assert.AreEqual(result, expression.Evaluate(processor));
            Assert.AreEqual(processor, leftExpression.Processor, "Left expression");
            Assert.AreEqual(processor, rightExpr.Processor, "Right expression");

            Assert.AreEqual(left, @operator.Left, "Left value");
            Assert.AreEqual(right, @operator.Right, "Right value");
        }

        [Test]
        [TestCase(7, 8)]
        [TestCase("7", "8")]
        [TestCase("7", 8)]
        public void UnaryExpressionTest(object value, object result)
        {
            var valueExpression = new ExpressionMock(() => value);
            var @operator = new OperatorMock(() => result);
            var expression = new Core.Cells.Expressions.UnaryExpression(@operator, valueExpression);

            var processor = TestExtensions.CreateProcessor();
            Assert.AreEqual(result, expression.Evaluate(processor));
            Assert.AreEqual(processor, valueExpression.Processor);
            Assert.AreEqual(value, @operator.Value);
        }

        [Test]
        [TestCase(5)]
        [TestCase("5")]
        public void UnaryExpressionTest(object value)
        {
            var expression = new Core.Cells.Expressions.ConstantExpression(value);
            Assert.AreEqual(value, expression.Evaluate(null));
        }

        [Test]
        [TestCase(9870)]
        [TestCase("check")]
        [TestCase(null)]
        public void CellReferenceExpressionTest(object value)
        {
            var cellReferenceExpression = new CellRefereceExpression(new CellAddress(0, 1));
            var processor = TestExtensions.CreateProcessor(
                cellReferenceExpression,
                new ConstantExpression(value));
            var result = cellReferenceExpression.Evaluate(processor);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void CellReferenceExpressionEvaluationExceptionTest()
        {
            Assert.That(() => CellReferenceExpressionTest(new ExpressionEvaluationException("error")), Throws.InstanceOf<ExpressionEvaluationException>());
        }

        [Test]
        public void CellReferenceExpressionGeneralExceptionTest()
        {
            Assert.That(() => CellReferenceExpressionTest(new Exception("error")), Throws.Exception);
        }
    }
}
