using System;
using System.Collections.Generic;
using System.IO;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Parsers.Operators;
using Spreadsheet.Core.Parsers.Tokenizers.Readers;
using static Spreadsheet.Core.Utils.SpesialCharactersSettings;

namespace Spreadsheet.Core.Parsers.Tokenizers
{
    internal class SpreadsheetStreamTokenizer : AbstractReaderWithPeekSupport<StreamReaderWithPeekSupport, Token>, ISpreadsheetTokenizer
    {
        private static readonly Dictionary<char, TokenType> TokenIdentifiers = new Dictionary<char, TokenType>
        {
            {ExpressionStart, TokenType.ExpressionStart},
            {LeftParanthesis, TokenType.LeftParenthesis},
            {RightParanthesis, TokenType.RightParenthesis}
        };

        public OperatorManager OperatorManager { get; }

        public SpreadsheetStreamTokenizer(Stream stream, OperatorManager operatorManager = null) : this(new StreamReader(stream), operatorManager)
        {
        }

        public SpreadsheetStreamTokenizer(TextReader stream, OperatorManager operatorManager = null) : base(new StreamReaderWithPeekSupport(stream))
        {
            OperatorManager = operatorManager ?? OperatorManager.Default;
        }

        protected override Token GetNextValue(StreamReaderWithPeekSupport source)
        {
            var peek = source.Peek();
            while (char.IsWhiteSpace(peek) && !IsSeparationCharacter(peek))
            {
                source.Read();
                peek = source.Peek();
            }

            if (peek == StreamEnd)
            {
                return new Token(TokenType.EndOfStream);
            }

            if (IsSeparationCharacter(peek) && peek != StreamEnd)
            {
                source.Read();
                if (peek == CarriageReturn && source.Peek() == RowSeparator)
                    source.Read();
                return new Token(TokenType.EndOfCell);
            }

            if (peek == StringStart)
                return ReadStringToken(source);

            if (char.IsDigit(peek))
                return ReadIntegerToken(source);

            if (IsColumnLetter(peek))
                return ReadCellReferenceToken(source);

            if (TokenIdentifiers.ContainsKey(peek))
            {
                source.Read();
                return new Token(TokenIdentifiers[peek]);
            }

            if (OperatorManager.Operators.ContainsKey(peek))
            {
                source.Read();
                return new Token(OperatorManager.Operators[peek]);
            }

            throw new ExpressionParsingException(source.ReadRemainExpression());
        }

        private Token ReadStringToken(StreamReaderWithPeekSupport source)
        {
            source.Read();
            return new Token(source.ReadRemainExpression());
        }

        private Token ReadIntegerToken(StreamReaderWithPeekSupport source)
        {
            return new Token(source.ReadInteger());
        }

        private Token ReadCellReferenceToken(StreamReaderWithPeekSupport source)
        {
            var column = source.ReadColumnNumber();
            var row = source.ReadInteger();
            //indexes is zero based, so we need to subtract 1 from current row and column values
            return new Token(new CellAddress(row - 1, column - 1));
        }
    }
}