using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using SpreadsheetProcessor.Cells;

namespace SpreadsheetProcessor.ExpressionParser
{
    public class ExpressionParser
    {
        private ExpressionTokenizer Tokenizer { get; } = new ExpressionTokenizer();

        public IExpression Parse(string expresion)
        {
            var tokens = Tokenizer.GetTokens(expresion).ToArray();
            switch (tokens.Length)
            {
                case 0:
                    return new ConstantExpression(new ExpressionValue(CellValueType.Nothing, null));
                case 1:
                    var token = tokens[0];
                    switch (token.Type)
                    {
                        case TokenType.Integer:
                            return ParseNumber(token);
                        case TokenType.String:
                            return ParseString(token);
                        default:
                            return InvalidContent(expresion, Resources.UnknownContentType);
                    }
                default:
                    return tokens[0].Type == TokenType.ExpressionStart 
                           ? ParseExpression(tokens)
                           : InvalidContent(expresion, string.Format(Resources.WrongExpressionStart, ParserSettings.ExpressionStart));
            }
        }

        private IExpression InvalidContent(string cellText, string additionMessage)
        {
            return new ConstantExpression(new ExpressionValue(CellValueType.Error, 
                string.Format(Resources.InvalidCellContent, cellText) + " " + additionMessage));
        }

        private IExpression ParseNumber(Token token)
        {
            int result;
            return int.TryParse(token.Value, out result) 
                ? new ConstantExpression(new ExpressionValue(CellValueType.Integer,  result)) 
                : new ConstantExpression(new ExpressionValue(CellValueType.Error, string.Format(Resources.FailedToParseNumber, token.Value)));
        }

        private IExpression ParseString(Token token)
        {
            return new ConstantExpression(new ExpressionValue(CellValueType.String, token.Value));
        }

        private IExpression ParseCellReference(Token token)
        {
            var referece = new CellAddress(token.Value);
            return new CellRefereceExpression(referece);
        }

        private IExpression ParseExpression(IReadOnlyList<Token> tokens, int index = 1)
        {
            IExpression left;
            switch (tokens[index].Type)
            {
                case TokenType.Integer:
                    left = ParseNumber(tokens[index]);
                    break;
                case TokenType.CellReference:
                    left = ParseCellReference(tokens[index]);
                    break;
                default:
                    return InvalidContent(tokens[index].Value, Resources.IdentifierExpected);
            }
            if (index + 1 >= tokens.Count)
                return left;

            index++;
            if (tokens[index].Type != TokenType.Operator)
                return InvalidContent(tokens[index].Value, Resources.OperatorExpected);

            //TODO check if operator is single character. Should be better solution
            var @operator = tokens[index].Value.Single();

            if (index + 1 >= tokens.Count)
                return InvalidContent(tokens[index].Value, Resources.IdentifierExpected);

            return new BinaryExpression(left, @operator, ParseExpression(tokens, index + 1));
        }
    }
}