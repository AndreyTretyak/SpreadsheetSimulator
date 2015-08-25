using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spreadsheet.Core
{

    public class SpreedsheatWriter : IDisposable
    {
        private readonly StreamWriter _stream;

        public SpreedsheatWriter(Stream stream)
        {
            _stream = new StreamWriter(stream);
        }

        public void WriteSpreedsheat(int columnsCount, IEnumerable<object> values)
        {
            //TODO: done for testing should be changed
            var index = 1;
            foreach (var value in values)
            {
                _stream.Write($"{value}\t");
                if (index++ % columnsCount == 0)
                    _stream.WriteLine();
            }      
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
