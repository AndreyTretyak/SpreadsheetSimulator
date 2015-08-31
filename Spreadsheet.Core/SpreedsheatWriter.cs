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
        private const string ErrorMarker = "#";

        private readonly TextWriter _streamWriter;

        public SpreedsheatWriter(TextWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public SpreedsheatWriter(Stream stream) : this(new StreamWriter(new BufferedStream(stream)))
        {
        }

        public void WriteSpreedsheat(SpreadsheetEvaluationResult result)
        {
            var index = 1;
            foreach (var value in result.Values)
            {
                var exception = value as Exception;
                if (exception != null)
                {
                    _streamWriter.Write(ErrorMarker);
                    _streamWriter.Write(exception.Message);
                }
                else
                {
                    _streamWriter.Write(value);
                }
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
