using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Spreadsheet.Core;
using Spreadsheet.Core.Cells.Expressions;
using Spreadsheet.Core.Parsers;
using Spreadsheet.Core.Parsers.Operators;
using Spreadsheet.Core.Parsers.Tokenizers;
using Spreadsheet.Core.Utils;
using Spreadsheet.Tests.Mocks;

using static Spreadsheet.Tests.Utils.TestExtensions;

namespace Spreadsheet.Tests;

[TestFixture]
public class SpreadsheetStreamParserTests
{
    private IEnumerable<IExpression> GetExpressions(params Token[] tokens)
    {
        var parser = new SpreadsheetStreamParser(new SpreadsheetTokenizerMock(tokens));
        IExpression expression;
        do
        {
            expression = parser.ReadExpression();
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

    [Test]
    public void TestEmptyCell()
    {
        var expression = Parse<ConstantExpression>(new Token(TokenType.EndOfCell));
        Assert.IsNull(expression.Value);
    }

    [Test]
    public void TestNothing()
    {
        Assert.IsNull(Parse());
    }

    [Test]
    public void WrongContentNotSeparatorErrorTest()
    {
        Assert.That(() => Parse(new Token(1), new Token(2)), Throws.InstanceOf<ExpressionParsingException>());
    }

    [Test]
    public void WrongContentUnexpectedTokenErrorTest()
    {
        Assert.That(() => Parse(new Token(TokenType.RightParenthesis)), Throws.InstanceOf<ExpressionParsingException>());
    }

    [Test]
    public void WorngExpressionTest()
    {
        Assert.That(() => Parse(new Token(TokenType.ExpressionStart),
              new Token(45),
              new Token(TokenType.LeftParenthesis)), Throws.InstanceOf<ExpressionParsingException>());
    }

    [Test]
    public void WorngExpressionNoCloseParentsisTest()
    {
        Assert.That(() => Parse(new Token(TokenType.ExpressionStart),
              new Token(TokenType.LeftParenthesis),
              new Token(63)), Throws.InstanceOf<ExpressionParsingException>());
    }

    [Test]
    public void WorngExpressionUnknownTokenTest()
    {
        Assert.That(() => Parse(new Token(TokenType.ExpressionStart),
              new Token(83),
              new Token(TokenType.ExpressionStart)), Throws.InstanceOf<ExpressionParsingException>());
    }

    [Test]
    [TestCase(123)]
    [TestCase(3712496)]
    [TestCase(-780)]
    public void TestInteger(int value)
    {
        Assert.AreEqual(value, Parse<ConstantExpression>(new Token(value)).Value);
    }

    [Test]
    [TestCase("test1 string")]
    [TestCase("'+test2")]
    [TestCase("12+test3")]
    public void TestStringValue(string expect)
    {
        Assert.AreEqual(expect, Parse<ConstantExpression>(new Token($"{expect}")).Value);
    }

    [Test]
    [TestCase("A13")]
    [TestCase("BVC197")]
    public void TestExpressionReference(string expect)
    {
        var expression = Parse<CellReferenceExpression>(
                            new Token(TokenType.ExpressionStart),
                            new Token(CellAddressConverter.FromString(expect)));
        Assert.AreEqual(expect, expression.Address.ToString());
    }

    [Test]
    [TestCase('+')]
    [TestCase('-')]
    public void UnaryOperationTest(char character)
    {
        var @operator = OperatorManager.Default.Operators[character];
        var expression = Parse<UnaryExpression>(
            new Token(TokenType.ExpressionStart),
            new Token(@operator),
            new Token(38));

        Assert.AreEqual(38, expression.Value.Evaluate(null));
        Assert.AreEqual(@operator, expression.Operation);
    }

    [Test]
    public void BinaryExpressionPriorityTest()
    {
        //=197-98/3");
        var expression = Parse<BinaryExpression>(
            new Token(TokenType.ExpressionStart),
            new Token(197),
            new Token(OperatorManager.Default.Operators['-']),
            new Token(98),
            new Token(OperatorManager.Default.Operators['/']),
            new Token(3));
        Assert.AreEqual(197, Cast<ConstantExpression>(expression.Left).Value);
        Assert.AreEqual('-', expression.Operation.OperatorCharacter);
        var right = Cast<BinaryExpression>(expression.Right);
        Assert.AreEqual(98, Cast<ConstantExpression>(right.Left).Value);
        Assert.AreEqual('/', right.Operation.OperatorCharacter);
        Assert.AreEqual(3, Cast<ConstantExpression>(right.Right).Value);
    }

    [Test]
    public void ParantsisTest()
    {
        //=18/(3-1);
        var expression = Parse<BinaryExpression>(
            new Token(TokenType.ExpressionStart),
            new Token(18),
            new Token(OperatorManager.Default.Operators['/']),
            new Token(TokenType.LeftParenthesis),
            new Token(3),
            new Token(OperatorManager.Default.Operators['-']),
            new Token(1),
            new Token(TokenType.RightParenthesis));
        Assert.AreEqual(18, Cast<ConstantExpression>(expression.Left).Value);
        Assert.AreEqual('/', expression.Operation.OperatorCharacter);
        var right = Cast<BinaryExpression>(expression.Right);
        Assert.AreEqual(3, Cast<ConstantExpression>(right.Left).Value);
        Assert.AreEqual('-', right.Operation.OperatorCharacter);
        Assert.AreEqual(1, Cast<ConstantExpression>(right.Right).Value);
    }
}
