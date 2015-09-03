using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spreadsheet.Core.Parsers.Tokenizers;

namespace Spreadsheet.Core
{

    public class SpreadsheetWriter : IDisposable
    {
        private readonly TextWriter _streamWriter;

        public SpreadsheetWriter(TextWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public SpreadsheetWriter(Stream stream) : this(new StreamWriter(new BufferedStream(stream)))
        {
        }

        public void WriteSpreedsheat(SpreadsheetProcessingResult result)
        {
            var index = 1;
            foreach (var value in result.Values)
            {
                var exception = value as Exception;
                if (exception != null)
                {
                    _streamWriter.Write(SpesialCharactersSettings.ErrorStartMarker);
                    var spreadsheetException = exception as SpreadsheetException;
                    _streamWriter.Write(spreadsheetException == null
                        ? exception.Message
                        : spreadsheetException.MessageWithCellCallStack);
                }
                else
                {
                    _streamWriter.Write(value);
                }
                _streamWriter.Write(SpesialCharactersSettings.CellSeparator);
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
