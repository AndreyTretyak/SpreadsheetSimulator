using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetProcessor;

namespace SpreadsheetSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = new SpreadsheetSource(File.Open("input.txt", FileMode.Open));
            var spreadsheet = new Spreadsheet(source);
            var processor = new SpreedsheetProcessor(spreadsheet);

            var result = processor.GetCellValue(new CellAddress("A1"));
        }
    }
}
