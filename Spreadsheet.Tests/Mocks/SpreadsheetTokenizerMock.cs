using Spreadsheet.Core.Parsers.Operators;
using Spreadsheet.Core.Parsers.Tokenizers;

namespace Spreadsheet.Tests.Mocks
{
    internal class SpreadsheetTokenizerMock : ISpreadsheetTokenizer
    {
        private readonly Token[] _tokens;

        public OperatorManager OperatorManager { get; }

        private static readonly Token EndToken = new Token(TokenType.EndOfStream);
        
        private int _index;
        
        public SpreadsheetTokenizerMock(params Token[] tokens)
        {
            _tokens = tokens;
            _index = 0;
            OperatorManager = OperatorManager.Default;
        }

        public Token Peek()
        {
            return _index < _tokens.Length ? _tokens[_index] : EndToken;
        }

        public Token Read()
        {
            return _index < _tokens.Length ? _tokens[_index++] : EndToken;
        }
    }
}