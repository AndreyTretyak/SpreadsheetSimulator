using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
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
            using (var tokenizer = new SpreadsheetStreamTokenizer(GenerateStreamFromString(text)))
            {
                Token token;
                do
                {
                    yield return token = tokenizer.Next();
                } while (token.Type != TokenType.EndOfStream);
            }
        }

        [Test]
        public void TokenizerComplexTest()
        {
            CollectionAssert.AreEqual(new []
            {
                new Token(TokenType.String,"test"),
                new Token(TokenType.EndOfExpression),
                new Token(TokenType.Integer,"19"),
                new Token(TokenType.EndOfExpression),
                new Token(TokenType.ExpressionStart,"="),
                new Token(TokenType.CellReference,"T31"),
                new Token(TokenType.EndOfExpression),
                new Token(TokenType.ExpressionStart,"="),
                new Token(TokenType.Integer,"14"),
                new Token(TokenType.Operator,"+"),
                new Token(TokenType.CellReference,"VK34"),
                new Token(TokenType.Operator,"/"),
                new Token(TokenType.Integer,"7"),
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
