namespace Spreadsheet.Core.Parsers.Tokenizers.Readers
{
    internal abstract class AbstractReaderWithPeekSupport<TSource, TResult> where TResult : struct
    {
        private TResult? _current;

        private readonly TSource _source;

        protected AbstractReaderWithPeekSupport(TSource source)
        {
            _source = source;
        }

        protected abstract TResult GetNextValue(TSource source);

        public TResult Peek()
        {
            if (_current.HasValue)
                return _current.Value;

            var value = GetNextValue(_source);
            _current = value;
            return value;
        }

        public TResult Read()
        {
            if (!_current.HasValue)
                return GetNextValue(_source);

            var value = _current.Value;
            _current = null;
            return value;
        }
    }
}