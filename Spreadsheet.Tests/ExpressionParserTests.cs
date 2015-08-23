using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SpreadsheetProcessor.Cells;
using SpreadsheetProcessor.ExpressionParsers;

namespace Spreadsheet.Tests
{
    [TestFixture]
    public class ExpressionParserTests
    {
        private ExpressionParser _parser;

        [SetUp]
        public void SetUp()
        {
            _parser = new ExpressionParser();
        }

        private T ParseExpresion<T>(string text)
        {
            var result = _parser.Parse(text);
            return Cast<T>(result);
        }

        private T Cast<T>(object result)
        {
            Assert.IsInstanceOf(typeof(T), result);
            return (T)result;
        }

        [Test]
        public void TestNothing()
        {
            Assert.IsNull(ParseExpresion<ConstantExpression>(string.Empty).Value);
        }

        [Test]
        public void TestInteger()
        {
            Assert.AreEqual(123, ParseExpresion<ConstantExpression>("123").Value);
        }

        [Test]
        [ExpectedException]
        public void TestHugeInteger()
        {
            //TODO: change after validation improvement
            ParseExpresion<ConstantExpression>("987654321123456789");
        }

        [Test]
        [TestCase("test1 string")]
        [TestCase("'+test2")]
        [TestCase("12+test3")]
        public void TestStringValue(string expect)
        {
            Assert.AreEqual(expect, ParseExpresion<ConstantExpression>($"'{expect}").Value);
        }
        
        [Test]
        [TestCase("A13")]
        [TestCase("BVC197")]
        public void TestExpressionReference(string expect)
        {
            Assert.AreEqual(expect, ParseExpresion<CellRefereceExpression>($"={expect}").Address.StringValue);
        }

        [Test]
        public void TestExpressionReference2()
        {
            Assert.AreEqual("BVC197", ParseExpresion<CellRefereceExpression>("=BVC197").Address.StringValue);
        }

        [Test]
        public void TestBinaryExpression1()
        {
            var expression = ParseExpresion<BinaryExpression>("=197-98/3");
            Assert.AreEqual(197, Cast<ConstantExpression>(expression.Left).Value);
            Assert.AreEqual("-", expression.Operation);

            var right = Cast<BinaryExpression>(expression.Right);
            Assert.AreEqual(98, Cast<ConstantExpression>(right.Left).Value);
            Assert.AreEqual("/", right.Operation);
            Assert.AreEqual(3, Cast<ConstantExpression>(right.Right).Value);
        }
    }
}
