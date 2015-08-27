using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Spreadsheet.Core.ExpressionParsers
{
    internal interface ISpreadsheetTokenizer
    {
        Token Peek();

        Token Next();
    }


    internal class SpreadsheetStreamTokenizer : ISpreadsheetTokenizer
    {
        private static readonly Dictionary<char, TokenType> TokenIdentifiers = new Dictionary<char, TokenType>
        {
            {ParserSettings.AdditionOperator[0], TokenType.Operator},
            {ParserSettings.SubtractionOperator[0], TokenType.Operator},
            {ParserSettings.MultiplicationOperator[0], TokenType.Operator},
            {ParserSettings.DivisionOperator[0], TokenType.Operator},
            {ParserSettings.ExpressionStart[0], TokenType.ExpressionStart},
            {ParserSettings.LeftParanthesis[0], TokenType.LeftParanthesis},
            {ParserSettings.RightParanthesis[0], TokenType.RightParanthesis}
        };

        private static readonly Func<char, bool> Number = char.IsDigit;

        private static readonly Func<char, bool> CellReference = char.IsLetterOrDigit;

        private static readonly Func<char, bool> RemainExpression = c => !char.IsWhiteSpace(c) && c != ParserSettings.StreamEndChar;

        private readonly StreamReader _stream;

        private Token? _currentToken = null;

        private readonly StringBuilder _stringBuilder;

        public SpreadsheetStreamTokenizer(Stream stream) : this(new StreamReader(stream))
        {
        }

        public SpreadsheetStreamTokenizer(StreamReader stream)
        {
            _stream = stream;
            _stringBuilder = new StringBuilder();
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

        private string CharsTill(Func<char, bool> selector)
        {
            _stringBuilder.Clear();
            while (selector(PeekChar())) _stringBuilder.Append(ReadChar());
            return _stringBuilder.ToString();
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

            if (peek == ParserSettings.StringStart[0])
                return ReadString();

            if (!TokenIdentifiers.ContainsKey(peek))
                return new Token(TokenType.Unknown, CharsTill(RemainExpression));

            ReadChar();
            return new Token(TokenIdentifiers[peek], peek.ToString());
        }

        private Token ReadNumber()
        {
            return new Token(TokenType.Integer, CharsTill(Number));
        }

        private Token ReadString()
        {
            ReadChar();
            return new Token(TokenType.String, CharsTill(RemainExpression));
        }

        private Token ReadCellReference()
        {
            return new Token(TokenType.CellReference, CharsTill(CellReference));
        }
    }
}