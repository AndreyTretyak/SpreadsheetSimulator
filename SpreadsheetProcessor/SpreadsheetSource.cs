using System;
using System.IO;
using System.Linq;

namespace SpreadsheetProcessor
{
    public class SpreadsheetSource
    {
        public CellAddress MaxAddress { get; }

        private string[][] Content { get; }

        public SpreadsheetSource(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                //TODO: need validation
                var size = reader.ReadLine().Split('\t').ToArray();
                MaxAddress = new CellAddress(int.Parse(size[0]), int.Parse(size[1]));
                Content = reader.ReadToEnd().Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Split(new [] {'\t'}, StringSplitOptions.RemoveEmptyEntries).ToArray())
                    .ToArray();
            }
        }

        public string GetCellContent(CellAddress cellAddress)
        {
            cellAddress.Validate(MaxAddress);
            return Content[cellAddress.Row][cellAddress.Column];
        }
    }
}