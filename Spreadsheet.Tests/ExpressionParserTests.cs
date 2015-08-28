﻿using System;
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
    internal class SpreadsheetTokenizerMock : ISpreadsheetTokenizer
    {
        private static readonly Token EndToken = new Token(TokenType.EndOfStream);

        private readonly Token[] _tokens;

        private int _index;
        
        public SpreadsheetTokenizerMock(params Token[] tokens)
        {
            _tokens = tokens;
            _index = 0;
            OperatorManager = OperatorManager.Default;
        }

        public Token Peek()
        {
            return _index < _tokens.Length ? _tokens[_index] : EndToken;
        }

        public Token Next()
        {
            return _index < _tokens.Length ? _tokens[_index++] : EndToken;
        }

        public OperatorManager OperatorManager { get; }
    }

    [TestFixture]
    public class ExpressionParserTests
    {
        private IEnumerable<IExpression> GetExpressions(params Token[] tokens)
        {
            var parser = new SpreadsheetStreamParser(new SpreadsheetTokenizerMock(tokens));
            IExpression expression;
            do
            {
                expression = parser.NextExpression();
                if (expression != null)
                    yield return expression;
            } while (expression != null);
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
            Assert.AreEqual(123, Parse<ConstantExpression>(new Token(123)).Value);
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
            var expression = Parse<CellRefereceExpression>(
                                new Token(TokenType.ExpressionStart), 
                                new Token(new CellAddress(expect)));
            //TODO: should be changed if redundant elements of tree will be removed from parser result
            Assert.AreEqual(expect, expression.Address.StringValue);
        }
        
        [Test]
        public void TestBinaryExpression1()
        {
            //var expression = Parse<BinaryExpression>("=197-98/3");
            var expression = Parse<BinaryExpression>(
                new Token(TokenType.ExpressionStart),
                new Token(197),
                new Token(OperatorManager.Default.Operators['-']),
                new Token(98),
                new Token(OperatorManager.Default.Operators['/']),
                new Token(3));
            //TODO: should be changed if redundant elements of tree will be removed from parser result
            Assert.AreEqual(197, Cast<ConstantExpression>(expression.Left).Value);
            Assert.AreEqual('-', expression.Operation.OperatorCharacter);
            var right = Cast<BinaryExpression>(expression.Right);
            Assert.AreEqual(98, Cast<ConstantExpression>(right.Left).Value);
            Assert.AreEqual('/', right.Operation.OperatorCharacter);
            Assert.AreEqual(3, Cast<ConstantExpression>(right.Right).Value);
        }
    }
}
