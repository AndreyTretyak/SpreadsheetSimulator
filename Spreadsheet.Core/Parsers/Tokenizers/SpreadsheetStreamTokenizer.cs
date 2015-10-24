using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Parsers.Operators;
using static Spreadsheet.Core.Parsers.Tokenizers.SpesialCharactersSettings;

namespace Spreadsheet.Core.Parsers.Tokenizers
{
    internal class SpreadsheetStreamTokenizer : ISpreadsheetTokenizer
    {
        private static readonly Dictionary<char, TokenType> TokenIdentifiers = new Dictionary<char, TokenType>
        {
            {ExpressionStart, TokenType.ExpressionStart},
            {LeftParanthesis, TokenType.LeftParenthesis},
            {RightParanthesis, TokenType.RightParenthesis}
        };

        public OperatorManager OperatorManager { get; }

        private readonly TextReader _stream;

        private readonly ReaderWithPeekSupport<Token> _tokenReader;

        private readonly ReaderWithPeekSupport<char> _charReader;

        private readonly StringBuilder _stringBuilder;

        public SpreadsheetStreamTokenizer(Stream stream, OperatorManager operatorManager = null) : this(new StreamReader(stream), operatorManager)
        {
        }

        public SpreadsheetStreamTokenizer(TextReader stream, OperatorManager operatorManager = null)
        {
            _stream = stream;
            _stringBuilder = new StringBuilder();
            OperatorManager = operatorManager ?? OperatorManager.Default;

            _charReader = new ReaderWithPeekSupport<char>(GetCharacterFromStream);
            _tokenReader = new ReaderWithPeekSupport<Token>(GetTokenFromStream);
        }

        public Token Peek() => _tokenReader.Peek();

        public Token Next() => _tokenReader.Next();

        private Token GetTokenFromStream()
        {
            var peek = PeekChar();
            while (char.IsWhiteSpace(peek) && !IsSeparationCharacter(peek))
            {
                NextChar();
                peek = PeekChar();
            }

            if (peek == StreamEnd)
            {
                return new Token(TokenType.EndOfStream);
            }

            if (IsSeparationCharacter(peek) && peek != StreamEnd)
            {
                NextChar();
                if (peek == CarriageReturn && PeekChar() == RowSeparator)
                    NextChar();
                return new Token(TokenType.EndOfCell);
            }

            if (peek == StringStart)
                return ReadStringToken();

            if (char.IsDigit(peek))
                return ReadIntegerToken();

            if (IsColumnLetter(peek)) 
                return ReadCellReferenceToken();

            if (TokenIdentifiers.ContainsKey(peek))
            {
                NextChar();
                return new Token(TokenIdentifiers[peek]);
            }

            if (OperatorManager.Operators.ContainsKey(peek))
            {
                NextChar();
                return new Token(OperatorManager.Operators[peek]);
            }

            throw new ExpressionParsingException(ReadRemainExpression());
        }

        private Token ReadStringToken()
        {
            NextChar();
            return new Token(ReadRemainExpression());
        }

        private Token ReadIntegerToken()
        {
            return new Token(ReadInteger());
        }

        public Token ReadCellReferenceToken()
        {
            var column = ReadColumnNumber();
            var row = ReadInteger();
            //indexes is zero based, so we need to subtract 1 from current row and column values
            return new Token(new CellAddress(row - 1, column - 1));
        }

        private string ReadRemainExpression()
        {
            _stringBuilder.Clear();
            while (!IsSeparationCharacter(PeekChar()))
            {
                _stringBuilder.Append(NextChar());
            }
            return _stringBuilder.ToString();
        }

        private int ReadInteger()
        {
            var value = 0;
            while (char.IsDigit(PeekChar()))
            {
                //check that next iteration will not make it bigger that MaxInt
                if ((uint)value > (int.MaxValue / 10))
                    throw new ExpressionParsingException(Resources.IntegerToBig);
                value = value * 10 + (NextChar() - '0');
            }
            return value;
        }

        private int ReadColumnNumber()
        {
            var value = 0;
            while (IsColumnLetter(PeekChar()))
            {
                //check that next iteration will not make it bigger that MaxInt
                if ((uint)value > (int.MaxValue / LettersUsedForColumnNumber))
                    throw new ExpressionParsingException(Resources.IntegerToBig);
                value = value * LettersUsedForColumnNumber + (char.ToUpper(NextChar()) - ColumnStartLetter + 1);
            }
            return value;
        }

        private char GetCharacterFromStream()
        {
            var result = _stream.Read();
            return (result == -1) ? StreamEnd : (char)result;
        }

        private char PeekChar() => _charReader.Peek();

        private char NextChar() => _charReader.Next();

        private static bool IsColumnLetter(char character)
        {
            character = char.ToUpper(character);
            //We subtract letter because LettersUsedForColumnNumber means 1
            return character >= ColumnStartLetter && character <= ColumnStartLetter + LettersUsedForColumnNumber - 1;
        }

        private static bool IsSeparationCharacter(char character)
        {
            return character == CellSeparator 
                || character == CarriageReturn 
                || character == RowSeparator
                || character == StreamEnd;
        }

        private class ReaderWithPeekSupport<T> where T : struct
        {
            private T? _current;

            private readonly Func<T> _getNextValue;

            public ReaderWithPeekSupport(Func<T> getNextValue)
            {
                _getNextValue = getNextValue;
            }

            public T Peek()
            {
                if (_current.HasValue)
                    return _current.Value;

                var value = _getNextValue();
                _current = value;
                return value;
            }

            public T Next()
            {
                if (!_current.HasValue)
                    return _getNextValue();

                var value = _current.Value;
                _current = null;
                return value;
            }
        }
    }
}