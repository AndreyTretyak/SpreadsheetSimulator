using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Spreadsheet.Core;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.ExpressionParsers;

namespace Spreadsheet.Tests
{
    [TestFixture]
    public class ExpressionStreamTokenizerTests
    {
        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        
        private IEnumerable<Token> GetTokens(string text)
        {
            var tokenizer = new SpreadsheetStreamTokenizer(GenerateStreamFromString(text));
            Token token;
            do
            {
                yield return token = tokenizer.Next();
            } while (token.Type != TokenType.EndOfStream);
        }

        [Test]
        [TestCase("A17", 16, 0)]
        [TestCase("C5", 4, 2)]
        [TestCase("A1", 0, 0)]
        [TestCase("A2", 1, 0)]
        [TestCase("B1", 0, 1)]
        [TestCase("BVC197", 196, 1926)]
        public void CellAddressTest(string address, int row, int column)
        {
            var token = GetTokens(address).First();
            Assert.AreEqual(token.Type, TokenType.CellReference);
            Assert.AreEqual(row, token.Address.Row, "row");
            Assert.AreEqual(column, token.Address.Column, "column");
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
        public void TokenizerComplexTest()
        {
            CollectionAssert.AreEqual(new []
            {
                new Token(TokenType.String,"test"),
                new Token(TokenType.EndOfExpression),
                new Token(19),
                new Token(TokenType.EndOfExpression),
                new Token(TokenType.ExpressionStart),
                new Token(new CellAddress("T31")),
                new Token(TokenType.EndOfExpression),
                new Token(TokenType.ExpressionStart),
                new Token(14),
                new Token(OperatorManager.Default.Operators['+']),
                new Token(new CellAddress("VK34")),
                new Token(OperatorManager.Default.Operators['/']),
                new Token(7),
                new Token(TokenType.EndOfStream),
            },
            GetTokens("'test\t19\n\r=T31\t=14+VK34/7"));
        }

        //[Test]
        //public void ParserTest()
        //{
        //    var result = GetExpressions("'test\t19\n\r=T31\t=14+VK34/7\t\n\r\t\r\n\t\n'p1\n=12*(2-5)/3-0+3").ToArray();
        //}
    }
}
