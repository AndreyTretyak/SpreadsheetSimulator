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
    [Obsolete]
    internal class ExpressionTokenizer
    {
        private static readonly Dictionary<string, TokenType> TokenIdentifiers = new Dictionary<string, TokenType>
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
                   : ParserSettings.StreamEndChar;
        }

        private char Next()
        {
            return _index < _expression.Length 
                   ? _expression[_index++] 
                   : ParserSettings.StreamEndChar;
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
            if (peek == ParserSettings.StreamEndChar) return null;

            var key = peek.ToString();
            if (key == ParserSettings.StringStart) return ReadString();
            if (TokenIdentifiers.ContainsKey(key))
            {
                Next();
                return new Token(TokenIdentifiers[key], peek.ToString());
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

    internal class ExpressionStreamTokenizer : IDisposable
    {
        private static readonly Dictionary<string, TokenType> TokenIdentifiers = new Dictionary<string, TokenType>
        {
            {ParserSettings.AdditionOperator, TokenType.Operator},
            {ParserSettings.SubtractionOperator, TokenType.Operator},
            {ParserSettings.MultiplicationOperator, TokenType.Operator},
            {ParserSettings.DivisionOperator, TokenType.Operator},
            {ParserSettings.ExpressionStart, TokenType.ExpressionStart},
            {ParserSettings.LeftParanthesis, TokenType.LeftParanthesis},
            {ParserSettings.RightParanthesis, TokenType.RightParanthesis}
        };

        private readonly StreamReader _stream;

        private Token? _currentToken = null;

        public ExpressionStreamTokenizer(Stream stream) : this(new StreamReader(stream))
        {
        }

        public ExpressionStreamTokenizer(StreamReader stream)
        {
            _stream = stream;
        }

        public Token Peek()
        {
            if (!_currentToken.HasValue)
                _currentToken = NextToken();
            return _currentToken.Value;
        }

        public Token Next()
        {
            if (!_currentToken.HasValue)
                _currentToken = NextToken();
            var value = _currentToken.Value;
            _currentToken = NextToken();
            return value;
        }

        private char PeekChar()
        {
            var result = _stream.Peek();
            if (result == -1)
                return ParserSettings.StreamEndChar;
            return (char) result;
        }

        private char ReadChar()
        {
            var result = _stream.Read();
            if (result == -1)
                return ParserSettings.StreamEndChar;
            return (char)result;
        }
        
        private string RemainExpression()
        {
            return CharsTill(c => !char.IsWhiteSpace(c) && c != ParserSettings.StreamEndChar);
        }

        private string CharsTill(Func<char, bool> selector)
        {
            var result = new StringBuilder();
            while (selector(PeekChar())) result.Append(ReadChar());
            return result.ToString();
        }

        private Token NextToken()
        {
            var peek = PeekChar();

            if (char.IsWhiteSpace(peek))
            {
                ReadChar();
                while (char.IsWhiteSpace(PeekChar())) ReadChar();
                return new Token(TokenType.EndOfExpression);
            }

            if (peek == ParserSettings.StreamEndChar)
                return new Token(TokenType.EndOfStream);

            if (char.IsDigit(peek))
                return ReadNumber();

            if (char.IsLetter(peek)) 
                return ReadCellReference();

            var key = peek.ToString();

            if (key == ParserSettings.StringStart)
                return ReadString();

            if (!TokenIdentifiers.ContainsKey(key))
                return new Token(TokenType.Unknown, RemainExpression());

            ReadChar();
            return new Token(TokenIdentifiers[key], key);
        }

        private Token ReadNumber()
        {
            return new Token(TokenType.Integer, CharsTill(char.IsDigit));
        }

        private Token ReadString()
        {
            ReadChar();
            return new Token(TokenType.String, RemainExpression());
        }

        private Token ReadCellReference()
        {
            return new Token(TokenType.CellReference, CharsTill(char.IsLetterOrDigit));
        }

        public void Dispose()
        {
            //_stream?.Dispose();
        }
    }
}
