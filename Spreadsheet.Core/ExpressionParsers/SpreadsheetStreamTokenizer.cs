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

        OperatorManager OperatorManager { get; }
    }


    internal class SpreadsheetStreamTokenizer : ISpreadsheetTokenizer
    {
        private static readonly Dictionary<char, TokenType> TokenIdentifiers = new Dictionary<char, TokenType>
        {
            {ParserSettings.ExpressionStart, TokenType.ExpressionStart},
            {ParserSettings.LeftParanthesis, TokenType.LeftParanthesis},
            {ParserSettings.RightParanthesis, TokenType.RightParanthesis}
        };

        public OperatorManager OperatorManager { get; }

        private readonly StreamReader _stream;

        private Token? _currentToken = null;

        private readonly StringBuilder _stringBuilder;

        public SpreadsheetStreamTokenizer(Stream stream, OperatorManager operatorManager = null) : this(new StreamReader(stream), operatorManager)
        {
        }

        public SpreadsheetStreamTokenizer(StreamReader stream, OperatorManager operatorManager = null)
        {
            _stream = stream;
            _stringBuilder = new StringBuilder();
            OperatorManager = operatorManager ?? OperatorManager.Default;
            
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
                return ReadIntegerToken();

            if (char.IsLetter(peek)) 
                return ReadCellReferenceToken();

            if (peek == ParserSettings.StringStart)
                return ReadStringToken();

            if (TokenIdentifiers.ContainsKey(peek))
            {
                ReadChar();
                return new Token(TokenIdentifiers[peek]);
            }

            if (OperatorManager.Operators.ContainsKey(peek))
            {
                ReadChar();
                return new Token(OperatorManager.Operators[peek]);
            }

            return new Token(TokenType.Unknown, ReadRemainExpression());
        }

        private Token ReadStringToken()
        {
            ReadChar();
            return new Token(TokenType.String, ReadRemainExpression());
        }

        private Token ReadIntegerToken()
        {
            return new Token(ReadInteger());
        }

        public Token ReadCellReferenceToken()
        {
            var column = 0;
            while (char.IsLetter(PeekChar()))
            {
                column = column * ParserSettings.LettersUsedForRowNumber + (ReadChar() - ParserSettings.RowNumberStartLetter + 1);
            }

            //indexes is zero based, so we need to subtract 1 from current row and column values
            return new Token(new CellAddress(ReadInteger() - 1, column - 1));
        }

        private int ReadInteger()
        {
            //manual integer reading to avoid memory allocation on string creation
            var value = 0;
            while (char.IsDigit(PeekChar()))
            {
                if ((uint)value > (0x7FFFFFFF / 10)) //check if next iteration make it bigger that MaxInt
                {
                    throw new ExpressionParsingException(Resources.IntegerToBig);
                }
                value = value * 10 + (ReadChar() - '0');
            }
            return value;
        }

        private string ReadRemainExpression()
        {
            _stringBuilder.Clear();
            var nextChar = PeekChar();
            while (nextChar != ParserSettings.CellSeparatorChar && nextChar != ParserSettings.StreamEndChar && !ParserSettings.RowSeparators.Contains(nextChar))
            {
                _stringBuilder.Append(ReadChar());
                nextChar = PeekChar();
            }
            return _stringBuilder.ToString();
        }
    }
}