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
        private readonly StreamWriter _streamWriter;

        public SpreedsheatWriter(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public SpreedsheatWriter(Stream stream) : this(new StreamWriter(new BufferedStream(stream)))
        {
        }

        public void WriteSpreedsheat(SpreadsheetEvaluationResult result)
        {
            //TODO: done for testing should be changed
            var index = 1;
            foreach (var value in result.Values)
            {
                _streamWriter.Write(value);
                _streamWriter.Write("\t");
                if (index++ % result.ColumnCount == 0)
                    _streamWriter.WriteLine();
            }
            _streamWriter.Flush();      
        }

        public void Dispose()
        {
            _streamWriter?.Dispose();
        }
    }
}
