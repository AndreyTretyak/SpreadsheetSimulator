﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Spreadsheet.Core.ExpressionParsers
{
    internal interface ISpreadsheetTokenizer : IDisposable
    {
        Token Peek();

        Token Next();
    }


    internal class SpreadsheetStreamTokenizer : ISpreadsheetTokenizer
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
        
        private string RemainExpression()
        {
            return CharsTill(c => !char.IsWhiteSpace(c) && c != ParserSettings.StreamEndChar);
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
            //TODO stream should be disposed
            //_stream?.Dispose();
        }
    }
}