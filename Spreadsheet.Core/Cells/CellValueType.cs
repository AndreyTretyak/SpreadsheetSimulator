using System;

namespace SpreadsheetProcessor.Cells
{
    [Flags]
    public enum CellValueType
    {
        Error = 1,
        Nothing = 2,
        Integer = 4,
        String = 8
    }
}