using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetProcessors;

namespace SpreadsheetProcessor.ExpressionParsers
{
    internal class ExpressionTokenizer
    {
        private static readonly Dictionary<char, TokenType> TokenIdentifiers = new Dictionary<char, TokenType>
        {
            {ParserSettings.AdditionOperator, TokenType.Operator},
            {ParserSettings.SubtractionOperator, TokenType.Operator},
            {ParserSettings.MultiplicationOperator, TokenType.Operator},
            {ParserSettings.DivisionOperator, TokenType.Operator},
            {ParserSettings.ExpressionStart, TokenType.ExpressionStart},
            {ParserSettings.LeftParanthesis, TokenType.LeftParanthesis},
            {ParserSettings.RightParanthesis, TokenType.RightParanthesis}
        };

        private string _expression;

        private int _index;

        private char Peek()
        {
            return _index < _expression.Length
                   ? _expression[_index]
                   : ParserSettings.ExpressionEndChar;
        }

        private char Next()
        {
            return _index < _expression.Length 
                   ? _expression[_index++] 
                   : ParserSettings.ExpressionEndChar;
        }

        private string RemainExpression()
        {
            var result = _expression.Substring(_index);
            _index = _expression.Length;
            return result;
        }

        private string CharsTill(Func<char, bool> selector)
        {
            var result = new string(_expression.Skip(_index).TakeWhile(selector).ToArray());
            _index += result.Length;
            return result;
        }

        public IEnumerable<Token> GetTokens(string expression)
        {
            _expression = expression;
            _index = 0;
            Token? token;
            do
            {
                token = NextToken();
                if (token.HasValue)
                    yield return token.Value;
            } while (token.HasValue);
        }

        private Token? NextToken()
        {
            while (char.IsWhiteSpace(Peek())) Next();

            var peek = Peek();
            if (char.IsDigit(peek)) return ReadNumber();
            if (char.IsLetter(peek)) return ReadCellReference();
            if (peek == ParserSettings.StringStart) return ReadString();
            if (peek == ParserSettings.ExpressionEndChar) return null;
            if (TokenIdentifiers.ContainsKey(peek))
            {
                Next();
                return new Token(TokenIdentifiers[peek], peek.ToString());
            }
            return new Token(TokenType.Unknown, RemainExpression());
        }

        private Token ReadNumber()
        {
            return new Token(TokenType.Integer, CharsTill(char.IsDigit));
        }

        private Token ReadString()
        {
            Next();
            return new Token(TokenType.String, RemainExpression());
        }

        private Token ReadCellReference()
        {
            return new Token(TokenType.CellReference, CharsTill(char.IsLetterOrDigit));
        }
    }
}
