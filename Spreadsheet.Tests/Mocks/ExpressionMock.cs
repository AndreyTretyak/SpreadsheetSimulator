using System;
using System.Collections.Generic;

using Spreadsheet.Core;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Cells.Expressions;

namespace Spreadsheet.Tests.Mocks;

public class ExpressionMock : IExpression
{
    private readonly Func<object> _expression;

    public int EvaluateCallCount { get; private set; }

    public SpreadsheetProcessor Processor { get; private set; }

    public ExpressionMock(Func<object> expression)
    {
        _expression = expression;
        EvaluateCallCount = 0;
    }

    public object Evaluate(SpreadsheetProcessor processor)
    {
        EvaluateCallCount++;
        Processor = processor;
        return _expression();
    }

    public IEnumerable<CellAddress> GetDependencies(SpreadsheetProcessor processor)
    {
        throw new NotImplementedException();
    }
}
