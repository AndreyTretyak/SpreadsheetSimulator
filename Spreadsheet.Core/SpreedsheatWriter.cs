using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetProcessor
{
    public class SpreedsheatWriter
    {
        private readonly SpreedsheetProcessor _processor;

        public SpreedsheatWriter(SpreedsheetProcessor processor)
        {
            _processor = processor;
        }

        public void Save(Stream output)
        {
            using (var stream = new StreamWriter(output))
            {
                for (var row = 0; row < _processor.MaxAddress.Row; row++)
                {
                    for (var col = 0; col < _processor.MaxAddress.Column; col++)
                    {
                        stream.Write(_processor.GetCellValue(new CellAddress(row, col)).StringRepresentation + "\t");
                    }
                    stream.WriteLine();
                }
            }
        }
    }
}
