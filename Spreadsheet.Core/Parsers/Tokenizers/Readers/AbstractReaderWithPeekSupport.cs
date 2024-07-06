namespace Spreadsheet.Core.Parsers.Tokenizers.Readers;

internal abstract class AbstractReaderWithPeekSupport<TResult> where TResult : struct
{
    private TResult? _current;

    protected abstract TResult GetNextValue();

    public TResult Peek()
    {
        if (_current.HasValue)
            return _current.Value;

        var value = GetNextValue();
        _current = value;
        return value;
    }

    public TResult Read()
    {
        if (!_current.HasValue)
            return GetNextValue();

        var value = _current.Value;
        _current = null;
        return value;
    }
}