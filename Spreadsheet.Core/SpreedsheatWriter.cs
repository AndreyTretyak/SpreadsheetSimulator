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
            _stream = new StreamWriter(new BufferedStream(stream));
        }

        public void WriteSpreedsheat(SpreadsheetEvaluationResult result)
        {
            //TODO: done for testing should be changed
            var index = 1;
            foreach (var value in result.Values)
            {
                _stream.Write(value);
                _stream.Write("\t");
                if (index++ % result.ColumnCount == 0)
                    _stream.WriteLine();
            }
            _stream.Flush();      
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
