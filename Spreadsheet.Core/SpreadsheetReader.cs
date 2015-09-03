using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Cells.Expressions;
using Spreadsheet.Core.Parsers;
using Spreadsheet.Core.Parsers.Tokenizers;
using static Spreadsheet.Core.Parsers.Tokenizers.SpesialCharactersSettings;

namespace Spreadsheet.Core
{
    public class SpreadsheetReader : IDisposable
    {
        private static readonly char[] SpreadsheetSizeSeparators = 
            {
                WhiteSpace,
                CellSeparator,
                CarriageReturn,
                RowSeparator
            };

        private readonly TextReader _textReader;

        private readonly Func<TextReader, ISpreadsheetParser> _parserFactory;

        public SpreadsheetReader(Stream stream) 
            : this(new StreamReader(stream))
        {
        }

        public SpreadsheetReader(TextReader textReader)
            : this(textReader, CreateParser)
        {
        }

        internal SpreadsheetReader(TextReader textReader, Func<TextReader,ISpreadsheetParser> parserFactory)
        {
            _textReader = textReader;
            _parserFactory = parserFactory;
        }

        public Spreadsheet ReadSpreadsheet()
        {
            var line = _textReader?.ReadLine();
            var size = line?.Split(SpreadsheetSizeSeparators, StringSplitOptions.RemoveEmptyEntries)
                            .ToArray();

            int maxRow;
            int maxColumn;
            if (size == null 
                || size.Length != 2
                || !int.TryParse(size[0], out maxRow)
                || maxRow < 0
                || !int.TryParse(size[1], out maxColumn)
                || maxColumn < 0)
                throw new SpreadsheatReadingException(string.Format(Resources.FailedToReadSpreadsheetSize, line));

            return new Spreadsheet(maxRow, maxColumn, GetCells(maxColumn, maxColumn * maxRow));
        }

        private IEnumerable<Cell> GetCells(int maxColumn, int cellCount)
        {
            var parser = _parserFactory(_textReader);
            for (var i = 0; i < cellCount; i++)
            {
                IExpression expression;
                try
                {
                    expression = parser.NextExpression();
                }
                catch (SpreadsheetException exception)
                {
                    expression = new ConstantExpression(exception);
                }
                yield return new Cell(new CellAddress(i / maxColumn, i % maxColumn), 
                                      expression ?? new ConstantExpression(null));
            }
            
        }

        public void Dispose()
        {
            _textReader?.Dispose();
        }

        private static ISpreadsheetParser CreateParser(TextReader s)
        {
            return new SpreadsheetStreamParser(new SpreadsheetStreamTokenizer(s));
        }
    }
}