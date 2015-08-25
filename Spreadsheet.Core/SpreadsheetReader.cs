using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using SpreadsheetProcessor.Cells;
using SpreadsheetProcessor.ExpressionParsers;

namespace SpreadsheetProcessor
{

    public class SpreadsheetReader
    {
        public ISpreadsheet GetSpreadsheet(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                //TODO: need validation
                var size = reader.ReadLine().Split(new []{' ', '\t', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).ToArray();
                
                var maxRow = int.Parse(size[0]);
                var maxColumn = int.Parse(size[1]);
                var cellCount = maxColumn * maxRow;
                var cells = new Cell[cellCount];
                using (var parser = new SpreadsheetStreamParser(new SpreadsheetStreamTokenizer(reader)))
                {
                    for (var i = 0; i < cellCount; i++)
                        cells[i] = new Cell(new CellAddress(i / maxColumn, i % maxColumn), parser.NextExpression());
                }
                return new Spreadsheet(maxRow, maxColumn, cells);
            }
        }
    }

}