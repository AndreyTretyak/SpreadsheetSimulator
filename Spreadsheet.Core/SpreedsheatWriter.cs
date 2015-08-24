using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetProcessor
{

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
