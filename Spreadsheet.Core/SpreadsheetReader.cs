using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Parsers;
using Spreadsheet.Core.Parsers.Tokenizers;
using static Spreadsheet.Core.Parsers.Tokenizers.TokenizerSettings;

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

        private readonly StreamReader _streamReader;

        private readonly Func<StreamReader, ISpreadsheetParser> _parserFactory;

        public SpreadsheetReader(Stream stream) 
            : this(new StreamReader(stream))
        {
        }

        public SpreadsheetReader(StreamReader streamReader)
            : this(streamReader, CreateParser)
        {
        }

        internal SpreadsheetReader(StreamReader streamReader, Func<StreamReader,ISpreadsheetParser> parserFactory)
        {
            _streamReader = streamReader;
            _parserFactory = parserFactory;
        }

        public Spreadsheet ReadSpreadsheet()
        {
            var size = _streamReader.ReadLine()
                                    ?.Split(SpreadsheetSizeSeparators, StringSplitOptions.RemoveEmptyEntries)
                                    .ToArray();

            int maxRow;
            int maxColumn;
            if (size == null 
                || !int.TryParse(size[0], out maxRow) 
                || !int.TryParse(size[1], out maxColumn))
                throw new SpreadsheatReadingException(Resources.FailedToReadSpreadsheetSize);

            return new Spreadsheet(maxRow, maxColumn, GetCells(maxColumn, maxColumn * maxRow));
        }

        private IEnumerable<Cell> GetCells(int maxColumn, int cellCount)
        {
            var parser = _parserFactory(_streamReader);
            for (var i = 0; i < cellCount; i++)
                yield return new Cell(new CellAddress(i / maxColumn, i % maxColumn), parser.NextExpression());
        }

        public void Dispose()
        {
            _streamReader?.Dispose();
        }

        private static ISpreadsheetParser CreateParser(StreamReader s)
        {
            return new SpreadsheetStreamParser(new SpreadsheetStreamTokenizer(s));
        }
    }
}