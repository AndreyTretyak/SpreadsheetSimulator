using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Spreadsheet.Core;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Parsers.Operators;
using Spreadsheet.Core.ProcessingStrategies;
using Spreadsheet.Core.Utils;

namespace Spreadsheet.Tests;


public class TestGenerator
{

}

public class ComplexTestGenerator
{
    private readonly int _row;
    private readonly int _column;
    private readonly Random _random;
    private readonly StringBuilder _builder;
    private readonly List<CellAddress> _calculatableCells;
    private readonly IOperator[] _operators;

    public ComplexTestGenerator(int row, int column, int seed = 1)
    {
        _row = row;
        _column = column;
        _random = new Random(seed);
        _builder = new StringBuilder(row * column * 3);
        _calculatableCells = new List<CellAddress>();
        _operators = OperatorManager.Default.Operators.Values.ToArray();
    }

    public string GenerateData()
    {
        _builder.Append(_row);
        _builder.Append(SpesialCharactersSettings.WhiteSpace);
        _builder.Append(_column);
        _builder.AppendLine();
        for (var i = 0; i < _row; i++)
        {
            for (var j = 0; j < _column; j++)
            {
                GenerateCell(i, j);
                _builder.Append(SpesialCharactersSettings.CellSeparator);
            }
            if (_random.NextDouble() < 0.5)
                _builder.Append(SpesialCharactersSettings.CarriageReturn);
            _builder.Append(SpesialCharactersSettings.RowSeparator);
        }
        return _builder.ToString();
    }

    private void GenerateCell(int currentRow, int currentColumn)
    {
        var cellType = _random.NextDouble();
        //string
        if (cellType < 0.25)
        {
            GenerateString();
        }
        //integer
        else if (cellType < 0.5)
        {
            _calculatableCells.Add(new CellAddress(currentRow, currentColumn));
            GenerateNumber();
        }
        //expression
        else if (cellType < 0.75)
        {
            GenerateExpression(currentRow, currentColumn);
        }
        //nothing
    }

    private void GenerateExpression(int currentRow, int currentColumn)
    {
        _calculatableCells.Add(new CellAddress(currentRow, currentColumn));
        _builder.Append(SpesialCharactersSettings.ExpressionStart);
        GenerateIdentifier();
        while (_random.NextDouble() < 0.5)
        {
            var @operator = _operators[_random.Next(_operators.Length)];
            _builder.Append(@operator.OperatorCharacter);
            GenerateIdentifier();
        }
    }

    private void GenerateIdentifier()
    {
        var type = _random.NextDouble();
        if (type < 0.5)
        {
            _builder.Append(_random.Next());
        }
        else
        {
            GenerateCellReference();
        }
    }

    private void GenerateNumber()
    {
        _builder.Append(_random.Next());
    }

    private void GenerateCellReference()
    {
        var refType = _random.NextDouble();
        //valid ref
        if (_calculatableCells.Any() && refType < 0.74)
        {
            _builder.Append(_calculatableCells[_random.Next(_calculatableCells.Count)]);
        }
        //possible valid
        else if (refType < 0.34)
        {
            _builder.Append(new CellAddress(_random.Next(_row), _random.Next(_column)));
        }
        //invalid ref
        else
        {
            _builder.Append(new CellAddress(_random.Next() + _row, _random.Next() + _column));
        }
    }

    private void GenerateString()
    {
        _builder.Append(SpesialCharactersSettings.StringStart);
        var stringSize = _random.Next(25);
        for (var i = 0; i < stringSize; i++)
        {
            var charType = _random.NextDouble();
            if (charType < 0.3)
            {
                _builder.Append((char)('A' + _random.Next(26)));
            }
            else if (charType < 0.9)
            {
                _builder.Append((char)('a' + _random.Next(26)));
            }
            else if (charType < 0.9)
            {
                _builder.Append((char)('0' + _random.Next(10)));
            }
            else
            {
                _builder.Append(SpesialCharactersSettings.WhiteSpace);
            }
        }
    }
}

[TestFixture]
public class ComplexTest
{
    [Test]
    public void ComplexTest1()
    {
        var data = new ComplexTestGenerator(100, 100).GenerateData();
        var evaluated = Evaluate(data);
        //TODO need way to test result
    }

    private static string Evaluate(string data)
    {
        using (var reader = new SpreadsheetReader(new StringReader(data)))
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new SpreadsheetWriter(ms))
                {
                    writer.WriteSpreadsheet(new SpreadsheetProcessor(reader.ReadSpreadsheet()).Evaluate(new ParallelProcessingStrategy()));
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
    }
}
