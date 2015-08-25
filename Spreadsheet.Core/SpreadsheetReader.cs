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
                var maxAddress = new CellAddress(int.Parse(size[0]), int.Parse(size[1]));

                var cellCount = maxAddress.Column * maxAddress.Row;
                var cells = new Cell[cellCount];
                using (var parser = new SpreadsheetStreamParser(new SpreadsheetStreamTokenizer(reader)))
                {
                    for (var i = 0; i < cellCount; i++)
                        cells[i] = new Cell(new CellAddress(i / maxAddress.Column, i % maxAddress.Column), parser.NextExpression());
                }
                return new Spreadsheet(maxAddress, cells);
            }
        }
    }

}