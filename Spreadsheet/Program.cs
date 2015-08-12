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

            var t = processor.GetCellValue(new CellAddress("A2"));

            var writer = new SpreedsheatWriter(processor);
            writer.Save(Console.OpenStandardOutput());
            Console.ReadKey();
        }
    }
}
