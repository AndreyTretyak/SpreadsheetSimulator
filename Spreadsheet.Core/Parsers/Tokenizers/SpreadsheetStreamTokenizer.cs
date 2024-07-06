using System.Collections.Generic;
using System.IO;

using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Parsers.Operators;
using Spreadsheet.Core.Parsers.Tokenizers.Readers;

using static Spreadsheet.Core.Utils.SpesialCharactersSettings;

namespace Spreadsheet.Core.Parsers.Tokenizers;

internal class SpreadsheetStreamTokenizer : AbstractReaderWithPeekSupport<Token>, ISpreadsheetTokenizer
{
    private static readonly Dictionary<char, TokenType> TokenIdentifiers = new Dictionary<char, TokenType>
    {
        {ExpressionStart, TokenType.ExpressionStart},
        {LeftParanthesis, TokenType.LeftParenthesis},
        {RightParanthesis, TokenType.RightParenthesis}
    };

    public OperatorManager OperatorManager { get; }

    private readonly StreamReaderWithPeekSupport _charReader;

    public SpreadsheetStreamTokenizer(Stream stream, OperatorManager operatorManager = null) : this(new StreamReader(stream), operatorManager)
    {
    }

    public SpreadsheetStreamTokenizer(TextReader stream, OperatorManager operatorManager = null)
    {
        _charReader = new StreamReaderWithPeekSupport(stream);
        OperatorManager = operatorManager ?? OperatorManager.Default;
    }

    protected override Token GetNextValue()
    {
        var peek = _charReader.Peek();
        while (char.IsWhiteSpace(peek) && !IsSeparationCharacter(peek))
        {
            _charReader.Read();
            peek = _charReader.Peek();
        }

        if (peek == StreamEnd)
        {
            return new Token(TokenType.EndOfStream);
        }

        if (IsSeparationCharacter(peek) && peek != StreamEnd)
        {
            _charReader.Read();
            if (peek == CarriageReturn && _charReader.Peek() == RowSeparator)
                _charReader.Read();
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
            _charReader.Read();
            return new Token(TokenIdentifiers[peek]);
        }

        if (OperatorManager.Operators.ContainsKey(peek))
        {
            _charReader.Read();
            return new Token(OperatorManager.Operators[peek]);
        }

        throw new ExpressionParsingException(_charReader.ReadRemainExpression());
    }

    private Token ReadStringToken()
    {
        _charReader.Read();
        return new Token(_charReader.ReadRemainExpression());
    }

    private Token ReadIntegerToken()
    {
        return new Token(_charReader.ReadInteger());
    }

    private Token ReadCellReferenceToken()
    {
        var column = _charReader.ReadColumnNumber();
        var row = _charReader.ReadInteger();
        //indexes is zero based, so we need to subtract 1 from current row and column values
        return new Token(new CellAddress(row - 1, column - 1));
    }
}