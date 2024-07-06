using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Spreadsheet.Core;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Parsers.Operators;
using Spreadsheet.Core.Parsers.Tokenizers;
using Spreadsheet.Core.Parsers.Tokenizers.Readers;
using Spreadsheet.Core.Utils;

namespace Spreadsheet.Tests
{
    [TestFixture]
    public class SpreadsheetStreamTokenizerTests
    {
        private ISpreadsheetTokenizer GetTokenizer(string text)
        {
            return new SpreadsheetStreamTokenizer(new StringReader(text));
        }

        private IEnumerable<Token> GetTokens(string text)
        {
            var tokenizer = GetTokenizer(text);
            Token token;
            do
            {
                yield return token = tokenizer.Read();
            } while (token.Type != TokenType.EndOfStream);
        }

        [Test]
        [TestCase("A17", 16, 0)]
        [TestCase("c5", 4, 2)]
        [TestCase("A1", 0, 0)]
        [TestCase("a2", 1, 0)]
        [TestCase("B1", 0, 1)]
        [TestCase("Bvc197", 196, 1926)]
        public void CellAddressTest(string address, int row, int column)
        {
            var token = GetTokens(address).First();
            Assert.AreEqual(token.Type, TokenType.CellReference);
            Assert.AreEqual(row, token.Address.Row, "Wrong row number");
            Assert.AreEqual(column, token.Address.Column, "Wrong column number");
        }

        [Test]
        [TestCase("179", 179)]
        [TestCase("0", 0)]
        [TestCase("009", 9)]
        [TestCase("2147483647", int.MaxValue)]
        public void IntegerTest(string text, int value)
        {
            var token = GetTokens(text).First();
            Assert.AreEqual(token.Type, TokenType.Integer);
            Assert.AreEqual(value, token.Integer);
        }

        [Test]
        [TestCase("987654321123456789")]
        [TestCase("A987654321123456789")]
        [TestCase("AMLJDSNKMNREWW9")]
        public void TestHugeInteger(string value)
        {
            Assert.That(() => { var token = GetTokens(value).First(); }, Throws.InstanceOf<ExpressionParsingException>());
        }

        [Test]
        [TestCase("hello world")]
        [TestCase("     1   1")]
        [TestCase("  test ")]
        [TestCase("'  ' 9 # @  '1")]
        [TestCase("")]
        public void StringTest(string expect)
        {
            var token = GetTokens($"{SpesialCharactersSettings.StringStart}{expect}").First();
            Assert.AreEqual(token.Type, TokenType.String);
            Assert.AreEqual(expect, token.String);
        }

        [Test]
        [TestCase("\t\t", 0)]
        [TestCase("1\t\t97", 1)]
        [TestCase("'test text\t\t13", 1)]
        [TestCase("'hello \t\t' world", 1)]
        [TestCase("8\t \t1", 1)]
        [TestCase("7\t     \t2", 1)]
        [TestCase("3\r \t4", 1)]
        [TestCase("5\t    \r\n4", 1)]
        [TestCase("6\n  \n0", 1)]
        public void EmptyCellTest(string text, int checkIndex)
        {
            var token = GetTokens(text).ToArray();
            Assert.AreEqual(TokenType.EndOfCell, token[checkIndex].Type);
            Assert.IsTrue(token[checkIndex + 1].Type == TokenType.EndOfCell
                          || token[checkIndex + 1].Type == TokenType.EndOfStream,
                          "Wrong close token for empty cell");
        }

        [Test]
        [TestCase(SpesialCharactersSettings.ExpressionStart, TokenType.ExpressionStart)]
        [TestCase(SpesialCharactersSettings.LeftParanthesis, TokenType.LeftParenthesis)]
        [TestCase(SpesialCharactersSettings.RightParanthesis, TokenType.RightParenthesis)]
        public void SpesialCharactersTest(char c, int tokenType)
        {
            var token = GetTokens(c.ToString()).First();
            Assert.AreEqual((TokenType)tokenType, token.Type);
        }

        [Test]
        public void OpereatorTests()
        {
            foreach (var @operator in OperatorManager.Default.Operators)
            {
                var token = GetTokens(@operator.Key.ToString()).First();
                Assert.AreEqual(TokenType.Operator, token.Type);
                Assert.AreEqual(@operator.Value, token.Operator);
            }
        }

        [Test]
        public void StreamEndTest()
        {
            var tokenizer = GetTokenizer(string.Empty);
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(TokenType.EndOfStream, tokenizer.Peek().Type);
                Assert.AreEqual(TokenType.EndOfStream, tokenizer.Read().Type);
            }
        }


        [Test]
        [TestCase("D0")]
        [TestCase("G")]
        public void InvalidCellReferenceTest(string text)
        {
            Assert.That(() => { var tokens = GetTokens(text).ToArray(); }, Throws.InstanceOf<InvalidCellAdressException>());
        }


        [Test]
        [TestCase("@")]
        [TestCase("178#1")]
        [TestCase("'tics\t%")]
        public void InvalidContentTest(string text)
        {
            Assert.That(() => { var tokens = GetTokens(text).ToArray(); }, Throws.InstanceOf<ExpressionParsingException>());
        }

        [Test]
        public void TokenizerComplexTest()
        {
            CollectionAssert.AreEqual(new[]
            {
                new Token("test"),
                new Token(TokenType.EndOfCell),
                new Token(19),
                new Token(TokenType.EndOfCell),
                new Token(TokenType.ExpressionStart),
                new Token(CellAddressConverter.FromString("T31")),
                new Token(TokenType.EndOfCell),
                new Token(TokenType.ExpressionStart),
                new Token(14),
                new Token(OperatorManager.Default.Operators['+']),
                new Token(CellAddressConverter.FromString("VK34")),
                new Token(OperatorManager.Default.Operators['/']),
                new Token(7),
                new Token(TokenType.EndOfStream),
            },
            GetTokens("'test\t19\r\n=T31\t=14+VK34/7"));
        }
    }
}
