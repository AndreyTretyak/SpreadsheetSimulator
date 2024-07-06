using System.IO;
using System.Text;
using Spreadsheet.Core.Utils;

namespace Spreadsheet.Core.Parsers.Tokenizers.Readers;

internal class StreamReaderWithPeekSupport : AbstractReaderWithPeekSupport<char>
{
    private readonly StringBuilder _stringBuilder;
    private readonly TextReader _stream;

    public StreamReaderWithPeekSupport(TextReader stream)
    {
        _stream = stream;
        _stringBuilder = new StringBuilder();
    }

    protected override char GetNextValue()
    {
        var result = _stream.Read();
        return (result == -1) ? SpesialCharactersSettings.StreamEnd : (char)result;
    }

    public string ReadRemainExpression()
    {
        _stringBuilder.Clear();
        while (!SpesialCharactersSettings.IsSeparationCharacter(Peek()))
        {
            _stringBuilder.Append(Read());
        }
        return _stringBuilder.ToString();
    }

    public int ReadInteger()
    {
        var value = 0;
        while (char.IsDigit(Peek()))
        {
            //check that next iteration will not make it bigger that MaxInt
            if ((uint)value > (int.MaxValue / 10))
                throw new ExpressionParsingException(Resources.IntegerToBig);
            value = value * 10 + (Read() - '0');
        }
        return value;
    }

    public int ReadColumnNumber()
    {
        var value = 0;
        while (SpesialCharactersSettings.IsColumnLetter(Peek()))
        {
            //check that next iteration will not make it bigger that MaxInt
            if ((uint)value > (int.MaxValue / SpesialCharactersSettings.LettersUsedForColumnNumber))
                throw new ExpressionParsingException(Resources.IntegerToBig);
            value = value * SpesialCharactersSettings.LettersUsedForColumnNumber + (char.ToUpper(Read()) - SpesialCharactersSettings.ColumnStartLetter + 1);
        }
        return value;
    }
}