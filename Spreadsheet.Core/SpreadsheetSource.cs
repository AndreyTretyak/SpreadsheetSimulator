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
    public class SpreadsheetSource
    {
        public CellAddress MaxAddress { get; }

        private readonly string[][] _content;

        public SpreadsheetSource(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                //TODO: need validation
                var size = reader.ReadLine().Split('\t').ToArray();
                MaxAddress = new CellAddress(int.Parse(size[0]), int.Parse(size[1]));
                _content = reader.ReadToEnd().Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Split(new [] {'\t'}, StringSplitOptions.RemoveEmptyEntries).ToArray())
                    .ToArray();
            }
        }

        public string GetCellContent(CellAddress cellAddress)
        {
            var validationResult = cellAddress.Validate(MaxAddress);
            if (!string.IsNullOrWhiteSpace(validationResult))
                throw new SpreadsheatReadingException(validationResult);
            return _content[cellAddress.Row][cellAddress.Column];
        }
    }

    public class SpreadsheetReader
    {
        private readonly ExpressionParser _parser;

        public SpreadsheetReader()
        {
            _parser = new ExpressionParser();
        }

        public ISpreadsheet GetSpreadsheet(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                //TODO: need validation
                var size = reader.ReadLine().Split(new []{'\t', ' '}, StringSplitOptions.RemoveEmptyEntries).ToArray();
                var maxAddress = new CellAddress(int.Parse(size[0]), int.Parse(size[1]));

                var enumerable = Enumerable.Empty<Cell>();
                var row = 0;
                while (!reader.EndOfStream)
                {
                    var column = 0;
                    var local = row; //needed to be sure that we will have correct value at execution moment
                    enumerable = enumerable.Concat(reader.ReadLine().Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(e => new Cell(new CellAddress(local, column++),  _parser.Parse(e))));
                    row++;
                }
                return new SpreadsheetMemoryCache(maxAddress, enumerable);
            }
        }
    }

}