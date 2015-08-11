namespace SpreadsheetProcessor.Cells
{
    public class ExpressionValue
    {
        public CellValueType Type { get; }

        public object Value { get; }

        public ExpressionValue(CellValueType type, object value)
        {
            Type = type;
            Value = value;
        }

        public string StringRepresentation => Value?.ToString() ?? Resources.Nothing;
    }
}