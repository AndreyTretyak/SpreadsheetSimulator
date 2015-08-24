using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SpreadsheetProcessor.Cells;
using SpreadsheetProcessor.ExpressionParsers;

namespace Spreadsheet.Tests
{
    internal class SpreadsheetTokenizerMock : ISpreadsheetTokenizer
    {
        private static readonly Token EndToken = new Token(TokenType.EndOfStream);

        private readonly Token[] _tokens;

        private int _index;
        
        public SpreadsheetTokenizerMock(params Token[] tokens)
        {
            _tokens = tokens;
            _index = 0;
        }

        public Token Peek()
        {
            return _index < _tokens.Length ? _tokens[_index] : EndToken;
        }

        public Token Next()
        {
            return _index < _tokens.Length ? _tokens[_index++] : EndToken;
        }

        public void Dispose() {}
    }

    [TestFixture]
    public class ExpressionParserTests
    {
        private IEnumerable<IExpression> GetExpressions(params Token[] tokens)
        {
            using (var parser = new SpreadsheetStreamParser(new SpreadsheetTokenizerMock(tokens)))
            {
                IExpression expression;
                do
                {
                    expression = parser.NextExpression();
                    if (expression != null)
                        yield return expression;
                } while (expression != null);
            }
        }

        private object Parse(params Token[] tokens)
        {
            //TODO: not optimal, may need optimization in future
            var list = tokens.ToList();
            list.Add(new Token(TokenType.EndOfStream));
            return GetExpressions(list.ToArray()).SingleOrDefault();
        }

        private T Parse<T>(params Token[] tokens)
        {
            return Cast<T>(Parse(tokens));
        }

        private T Cast<T>(object result)
        {
            Assert.IsInstanceOf(typeof(T), result);
            return (T)result;
        }

        [Test]
        public void TestNothing()
        {
            Assert.IsNull(Parse());
        }

        [Test]
        public void TestInteger()
        {
            Assert.AreEqual(123, Parse<ConstantExpression>(new Token(TokenType.Integer, "123")).Value);
        }

        [Test]
        [ExpectedException(typeof(ExpressionParsingException))]
        public void TestHugeInteger()
        {
            Parse<ConstantExpression>(new Token(TokenType.Integer, "987654321123456789"));
        }

        [Test]
        [TestCase("test1 string")]
        [TestCase("'+test2")]
        [TestCase("12+test3")]
        public void TestStringValue(string expect)
        {
            Assert.AreEqual(expect, Parse<ConstantExpression>(new Token(TokenType.String, $"{expect}")).Value);
        }
        
        [Test]
        [TestCase("A13")]
        [TestCase("BVC197")]
        public void TestExpressionReference(string expect)
        {
            var expression = Parse<BinaryExpression>(
                                new Token(TokenType.ExpressionStart), 
                                new Token(TokenType.CellReference, expect));
            //TODO: should be changed if redundant elements of tree will be removed from parser result
            Assert.AreEqual(expect, Cast<CellRefereceExpression>(Cast<BinaryExpression>(expression.Left).Left).Address.StringValue);
        }
        
        [Test]
        public void TestBinaryExpression1()
        {
            //var expression = Parse<BinaryExpression>("=197-98/3");
            var expression = Parse<BinaryExpression>(
                new Token(TokenType.ExpressionStart),
                new Token(TokenType.Integer, "197"),
                new Token(TokenType.Operator, "-"),
                new Token(TokenType.Integer, "98"),
                new Token(TokenType.Operator, "/"),
                new Token(TokenType.Integer, "3"));
            //TODO: should be changed if redundant elements of tree will be removed from parser result
            Assert.AreEqual(197, Cast<ConstantExpression>(Cast<BinaryExpression>(expression.Left).Left).Value);
            Assert.AreEqual("-", expression.Operation);
            var right = Cast<BinaryExpression>(expression.Right);
            Assert.AreEqual(98, Cast<ConstantExpression>(right.Left).Value);
            Assert.AreEqual("/", right.Operation);
            Assert.AreEqual(3, Cast<ConstantExpression>(right.Right).Value);
        }
    }
}
