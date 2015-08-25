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
            var spreadsheet = new SpreadsheetReader().GetSpreadsheet(stream);
            var processor = new SpreadsheetProcessor.SpreadsheetProcessor(spreadsheet);
            var result = processor.Evaluate(new ParallelEvaluationStrategy());
            using (var write = new SpreedsheatWriter(Console.OpenStandardOutput()))
            {
                write.WriteSpreedsheat(spreadsheet.ColumnCount, result);
            }
            Console.ReadKey();
        }
    }
}
