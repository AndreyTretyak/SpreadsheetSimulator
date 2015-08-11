using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetProcessor.ExpressionParser
{
    internal enum TokenType
    {
        ExpressionStart,
        StringStart,
        CellReference,
        Integer,
        String,
        Operator,
        Unknown
    }

    internal struct Token
    {
        private TokenType Type { get; }
        private string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return Type + "|" + Value;
        }
    }
    
    internal class ExpressionTokenizer
    {
        private static readonly Dictionary<char, TokenType> TokenIdentifiers = new Dictionary<char, TokenType>
        {
            {ParserSettings.AdditionOperator, TokenType.Operator},
            {ParserSettings.SubtractionOperator, TokenType.Operator},
            {ParserSettings.MultiplicationOperator, TokenType.Operator},
            {ParserSettings.DivisionOperator, TokenType.Operator},
            {ParserSettings.ExpressionStart, TokenType.ExpressionStart}
        };

        private string Expression { get; set; }

        private int Index { get; set;  }

        private char Peek()
        {
            return Index < Expression.Length
                   ? Expression[Index]
                   : ParserSettings.ExpressionEndChar;
        }

        private char Next()
        {
            return Index < Expression.Length 
                   ? Expression[Index++] 
                   : ParserSettings.ExpressionEndChar;
        }

        private string RemainExpression()
        {
            var result = Expression.Substring(Index);
            Index = Expression.Length;
            return result;
        }

        private string CharsTill(Func<char, bool> selector)
        {
            var result = new string(Expression.Skip(Index).TakeWhile(selector).ToArray());
            Index += result.Length;
            return result;
        }

        public IEnumerable<Token> GetTokens(string expression)
        {
            Expression = expression;
            Index = 0;
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
