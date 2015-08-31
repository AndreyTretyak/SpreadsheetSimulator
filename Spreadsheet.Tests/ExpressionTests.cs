using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Spreadsheet.Core.Cells;
using Spreadsheet.Tests.Mocks;
using Spreadsheet.Core.Cells.Expressions;

namespace Spreadsheet.Tests
{
    [TestFixture]
    public class ExpressionTests
    {
        [Test]
        [TestCase(1,2,3)]
        [TestCase("1", "2", "3")]
        [TestCase("1", 2, "3")]
        public void BinaryExpressionTest(object left, object right, object result)
        {
            var leftExpression = new ExpressionMock(() => left);
            var rightExpr = new ExpressionMock(() => right);
            var @operator = new OperatorMock(() => result);
            var expression = new Core.Cells.Expressions.BinaryExpression(leftExpression, @operator, rightExpr);

            var processor = MockCreator.CreateProcessor();
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

            var processor = MockCreator.CreateProcessor();
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

        //[Test]
        //public void CellReferenceExpressionTest()
        //{
        //    var expression = new CellRefereceExpression(new CellAddress(0, 0));
        //    var processor = MockCreator.CreateProcessor();
        //    expression.Evaluate(processor);
        //    throw new NotImplementedException();
        //}
    }
}
