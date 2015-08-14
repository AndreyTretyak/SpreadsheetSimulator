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
            Assert.AreEqual(CellValueType.Nothing, ParseExpresion<ConstantExpression>(string.Empty).Value.Type);
        }

        [Test]
        public void TestInteger()
        {
            Assert.AreEqual(123, ParseExpresion<ConstantExpression>("123").Value.Value);
        }

        [Test]
        public void TestHugeInteger()
        {
            Assert.AreEqual(CellValueType.Error, ParseExpresion<ConstantExpression>("987654321123456789").Value.Type);
        }

        [Test]
        public void TestString1()
        {
            Assert.AreEqual("test1 string", ParseExpresion<ConstantExpression>("'test1 string").Value.Value);
        }

        [Test]
        public void TestString2()
        {
            Assert.AreEqual("+test2", ParseExpresion<ConstantExpression>("'+test2").Value.Value);
        }

        [Test]
        public void TestString3()
        {
            Assert.AreEqual("12+test3", ParseExpresion<ConstantExpression>("'12+test3").Value.Value);
        }

        [Test]
        public void TestExpressionReference1()
        {
            Assert.AreEqual("A13", ParseExpresion<CellRefereceExpression>("=A13").Address.StringValue);
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
            Assert.AreEqual(197, Cast<ConstantExpression>(expression.Left).Value.Value);
            Assert.AreEqual('-', expression.Operation);

            var right = Cast<BinaryExpression>(expression.Right);
            Assert.AreEqual(98, Cast<ConstantExpression>(right.Left).Value.Value);
            Assert.AreEqual('/', right.Operation);
            Assert.AreEqual(3, Cast<ConstantExpression>(right.Right).Value.Value);
        }
    }
}
