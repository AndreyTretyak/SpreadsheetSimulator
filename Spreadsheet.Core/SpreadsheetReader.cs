using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.ExpressionParsers;

namespace Spreadsheet.Core
{

    public class SpreadsheetReader : IDisposable
    {
        private readonly StreamReader _streamReader;

        public SpreadsheetReader(Stream stream) : this(new StreamReader(stream))
        {
        }

        public SpreadsheetReader(StreamReader streamReader)
        {
            _streamReader = streamReader;
        }

        public Spreadsheet ReadSpreadsheet()
        {
            //TODO: need validation
            var size = _streamReader.ReadLine()
                                    .Split(new []{' ', '\t', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                                    .ToArray();
                
            var maxRow = int.Parse(size[0]);
            var maxColumn = int.Parse(size[1]);
            return new Spreadsheet(maxRow, maxColumn, GetCells(maxColumn, maxColumn * maxRow));
        }

        private IEnumerable<Cell> GetCells(int maxColumn, int cellCount)
        {
            var parser = new SpreadsheetStreamParser(new SpreadsheetStreamTokenizer(_streamReader));
            for (var i = 0; i < cellCount; i++)
                yield return new Cell(new CellAddress(i / maxColumn, i % maxColumn), parser.NextExpression());
        }

        public void Dispose()
        {
            _streamReader?.Dispose();
        }
    }
}