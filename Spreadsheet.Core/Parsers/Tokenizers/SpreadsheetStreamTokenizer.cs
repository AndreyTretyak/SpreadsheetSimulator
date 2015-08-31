using System.Collections.Generic;
using System.IO;
using System.Text;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Parsers.Operators;
using static Spreadsheet.Core.Parsers.Tokenizers.TokenizerSettings;

namespace Spreadsheet.Core.Parsers.Tokenizers
{
    internal class SpreadsheetStreamTokenizer : ISpreadsheetTokenizer
    {
        private static readonly Dictionary<char, TokenType> TokenIdentifiers = new Dictionary<char, TokenType>
        {
            {ExpressionStart, TokenType.ExpressionStart},
            {LeftParanthesis, TokenType.LeftParanthesis},
            {RightParanthesis, TokenType.RightParanthesis}
        };

        public OperatorManager OperatorManager { get; }

        private readonly TextReader _stream;

        private Token? _currentToken = null;

        private readonly StringBuilder _stringBuilder;

        public SpreadsheetStreamTokenizer(Stream stream, OperatorManager operatorManager = null) : this(new StreamReader(stream), operatorManager)
        {
        }

        public SpreadsheetStreamTokenizer(TextReader stream, OperatorManager operatorManager = null)
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
                return StreamEnd;
            return (char) result;
        }

        private char ReadChar()
        {
            var result = _stream.Read();
            if (result == -1)
                return StreamEnd;
            return (char)result;
        }

        private Token NextToken()
        {
            var peek = PeekChar();
            while (char.IsWhiteSpace(peek) && !IsSeparationCharacter(peek))
            {
                ReadChar();
                peek = PeekChar();
            }

            if (peek == StreamEnd)
            {
                return new Token(TokenType.EndOfStream);
            }
            

            if (IsSeparationCharacter(peek) && peek != StreamEnd)
            {
                ReadChar();
                if (peek == CarriageReturn && PeekChar() == RowSeparator)
                    ReadChar();
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
                ReadChar();
                return new Token(TokenIdentifiers[peek]);
            }

            if (OperatorManager.Operators.ContainsKey(peek))
            {
                ReadChar();
                return new Token(OperatorManager.Operators[peek]);
            }

            throw new ExpressionParsingException(ReadRemainExpression());
        }

        private Token ReadStringToken()
        {
            ReadChar();
            return new Token(ReadRemainExpression());
        }

        private Token ReadIntegerToken()
        {
            return new Token(ReadInteger());
        }

        public Token ReadCellReferenceToken()
        {
            var column = ReadColumn();
            var row = ReadInteger();
            //indexes is zero based, so we need to subtract 1 from current row and column values
            return new Token(new CellAddress(row - 1, column - 1));
        }

        private string ReadRemainExpression()
        {
            _stringBuilder.Clear();
            while (!IsSeparationCharacter(PeekChar()))
            {
                _stringBuilder.Append(ReadChar());
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
                value = value * 10 + (ReadChar() - '0');
            }
            return value;
        }

        private int ReadColumn()
        {
            var value = 0;
            while (IsColumnLetter(PeekChar()))
            {
                //check that next iteration will not make it bigger that MaxInt
                if ((uint)value > (int.MaxValue / LettersUsedForColumnNumber))
                    throw new ExpressionParsingException(Resources.IntegerToBig);
                value = value * LettersUsedForColumnNumber + (char.ToUpper(ReadChar()) - ColumnStartLetter + 1);
            }
            return value;
        }

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
    }
}