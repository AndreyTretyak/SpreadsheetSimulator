using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SpreadsheetProcessor.ExpressionParser;

namespace Spreadsheet.Tests
{
    [TestFixture]
    public class ExpressionTokenizerTests
    {
        private ExpressionTokenizer Tokenizer;
        [SetUp]
        public void SetUp()
        {
            Tokenizer = new ExpressionTokenizer();
        }

        private void AssertResult(string test, IEnumerable<Token> expectedResult)
        {
            CollectionAssert.AreEqual(expectedResult, Tokenizer.GetTokens(test));
        }

        [Test]
        public void StringTest()
        {
            AssertResult(@"'some string 12 / = 5", new []
            {
                new Token(TokenType.String, @"some string 12 / = 5"), 
            });
        }

        [Test]
        public void IntegerTest()
        {
            AssertResult("987123456", new[]
            {
                new Token(TokenType.Integer, "987123456"),
            });
        }

        [Test]
        public void CellReferenceTest()
        {
            AssertResult("A61", new[]
            {
                new Token(TokenType.CellReference, "A61"),
            });
        }

        [Test]
        public void EmptyTest()
        {
            AssertResult(string.Empty, new Token[0]);
        }

        [Test]
        public void UnknownCharTest1()
        {
            AssertResult("@some content", new []
            {
                new Token(TokenType.Unknown, "@some content")
            });
        }

        [Test]
        public void UnknownCharTest2()
        {
            AssertResult("=D12-#12some other content", new []
            {
                new Token(TokenType.ExpressionStart, "="),
                new Token(TokenType.CellReference, "D12"),
                new Token(TokenType.Operator, "-"),
                new Token(TokenType.Unknown, "#12some other content")
            });
        }

        [Test]
        public void ExpressionTest()
        {
            AssertResult("=V1-190+G670/0*56", new[]
            {
                new Token(TokenType.ExpressionStart, "="),
                new Token(TokenType.CellReference,   "V1"),
                new Token(TokenType.Operator, "-"),
                new Token(TokenType.Integer, "190"),
                new Token(TokenType.Operator, "+"),
                new Token(TokenType.CellReference, "G670"),
                new Token(TokenType.Operator, "/"),
                new Token(TokenType.Integer, "0"),
                new Token(TokenType.Operator, "*"),
                new Token(TokenType.Integer, "56")
            });
        }
    }
}
