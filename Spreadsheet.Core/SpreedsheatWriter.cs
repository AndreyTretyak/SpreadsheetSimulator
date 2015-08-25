using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetProcessor
{

    public class SpreedsheatWriter : IDisposable
    {
        private readonly StreamWriter _stream;

        public SpreedsheatWriter(Stream stream)
        {
            _stream = new StreamWriter(stream);
        }

        public void WriteSpreedsheat(ISpreadsheet spreadsheat)
        {
            //TODO: done for testing should be changed
            var index = 1;
            foreach (var cell in spreadsheat)
            {
                _stream.Write($"{spreadsheat.GetCellValue(cell.Address)}\t");
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
