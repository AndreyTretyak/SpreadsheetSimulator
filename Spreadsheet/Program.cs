using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spreadsheet.Core;

namespace SpreadsheetSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var stream = File.Open("input.txt", FileMode.Open);
            using (var reader = new SpreadsheetReader(stream))
            {
                var spreadsheet = reader.ReadSpreadsheet();
                var processor = new SpreadsheetProcessor(spreadsheet);
                var result = processor.Evaluate(new ParallelProcessingStrategy());
                using (var write = new SpreedsheatWriter(Console.OpenStandardOutput()))
                {
                    write.WriteSpreedsheat(result);
                }
            }
            Console.ReadKey();
        }
    }
}
