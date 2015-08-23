using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetProcessor
{
    [Obsolete]
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
                        stream.Write(_processor.GetCellValue(new CellAddress(row, col)) + "\t");
                    }
                    stream.WriteLine();
                }
            }
        }
    }

    public class SpreedsheatResultWriter : IDisposable
    {
        private readonly StreamWriter _stream;

        public SpreedsheatResultWriter(Stream stream)
        {
            _stream = new StreamWriter(stream);
        }

        public void WriteSpreedsheat(ISpreadsheet spreadsheat, IProcessingStrategy strategy)
        {
            //TODO: done for testing should be changed
            var index = 1;
            foreach (var value in strategy.Process(spreadsheat))
            {
                _stream.Write(value + "\t");
                if (index++ % spreadsheat.MaxAddress.Column == 0)
                    _stream.WriteLine();
            }      
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
