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
            var stream = File.Open("input.txt", FileMode.Open);

            //var source = new SpreadsheetSource(stream);
            //var spreadsheet = new Spreadsheet(source);

            var spreadsheet = new SpreadsheetReader().GetSpreadsheet(stream);
            using (var write = new SpreedsheatResultWriter(Console.OpenStandardOutput()))
            {
                write.WriteSpreedsheat(spreadsheet, new ParallelProcessingStrategy());
            }
            
            /*
            var processor = new SpreedsheetProcessor(spreadsheet);
            var writer = new SpreedsheatWriter(processor);
            writer.Save(Console.OpenStandardOutput());
            */
            Console.ReadKey();
        }
    }
}
