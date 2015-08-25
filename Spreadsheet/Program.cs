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
            spreadsheet = new SpreadsheetEvaluator().Evaluate(spreadsheet, new SimpleEvaluationStrategy());

            using (var write = new SpreedsheatWriter(Console.OpenStandardOutput()))
            {
                write.WriteSpreedsheat(spreadsheet);
            }
            
            Console.ReadKey();
        }
    }
}
