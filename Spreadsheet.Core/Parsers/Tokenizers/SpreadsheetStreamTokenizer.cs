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
                return StreamEndChar;
            return (char) result;
        }

        private char ReadChar()
        {
            var result = _stream.Read();
            if (result == -1)
                return StreamEndChar;
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

            if (peek == StreamEndChar)
                return new Token(TokenType.EndOfStream);

            if (char.IsDigit(peek))
                return ReadIntegerToken();

            if (char.IsLetter(peek)) 
                return ReadCellReferenceToken();

            if (peek == StringStart)
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
            var column = ReadInteger(LettersUsedForRowNumber, RowNumberStartLetter, 1);
            var row = ReadInteger();
            //indexes is zero based, so we need to subtract 1 from current row and column values
            return new Token(new CellAddress(row - 1, column - 1));
        }

        private string ReadRemainExpression()
        {
            _stringBuilder.Clear();
            var nextChar = PeekChar();
            while (nextChar != CellSeparator
                && nextChar != CarriageReturn
                && nextChar != RowSeparator
                && nextChar != StreamEndChar)
            {
                _stringBuilder.Append(ReadChar());
                nextChar = PeekChar();
            }
            return _stringBuilder.ToString();
        }

        /// <summary>
        /// Manual integer reading to avoid memory allocation on string creation.
        /// Reads integer from input stream in given system of calculation, by default use decimal.
        /// </summary>
        /// <param name="systemBase">Base of calculation system.</param>
        /// <param name="startChar">
        /// Character with minimal value. 
        /// Range of allowed characters is between startChar and startChar plus systemBase minus shift.
        /// </param>
        /// <param name="shift">
        /// Difference between start character and character that represent zero. 
        /// If start character 'A' means 1 than swift should be one, 
        /// for '0' that already means 0 shift is zero.  
        /// </param>
        /// <returns>Read int value as regular integer.</returns>
        private int ReadInteger(int systemBase = 10, char startChar = '0', int shift = 0)
        {
            var zeroChar = startChar - shift;
            var maxAllowedChar = systemBase + zeroChar;

            var value = 0;
            var peek = PeekChar();
            while (peek >= startChar && peek <= maxAllowedChar)
            {
                //check that next iteration will not make it bigger that MaxInt
                if ((uint)value > (int.MaxValue / systemBase))
                {
                    throw new ExpressionParsingException(Resources.IntegerToBig);
                }
                value = value * systemBase + (ReadChar() - zeroChar);
                peek = PeekChar();
            }
            return value;
        }
    }
}