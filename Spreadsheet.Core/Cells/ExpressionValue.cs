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

        //public ExpressionValue() : this(CellValueType.Nothing, null)
        //{
        //}

        //public ExpressionValue(int value) : this(CellValueType.Integer, value)
        //{
        //}

        //public ExpressionValue(bool isError, string value) : this(isError ? CellValueType.Error : CellValueType.String, value)
        //{
        //}
        
        public string StringRepresentation => Value?.ToString() ?? Resources.Nothing;

        public override string ToString() => StringRepresentation;
    }
}