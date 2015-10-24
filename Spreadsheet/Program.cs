using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spreadsheet.Core;
using Spreadsheet.Core.ProcessingStrategies;

namespace SpreadsheetSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var isConsoleInput = args.Length < 1;
            var isConsoleOutput = args.Length < 2;
            try
            {
                var input = isConsoleInput ? Console.OpenStandardInput() : File.Open(args[0], FileMode.Open);
                var output = isConsoleOutput ? Console.OpenStandardOutput() : File.Create(args[1]);
                using (var reader = new SpreadsheetReader(input))
                {
                    Console.WriteLine(isConsoleInput ? Resources.EnterTable : Resources.ReadingTable);
                    var spreadsheet = reader.ReadSpreadsheet();

                    Console.WriteLine(Resources.Processing);
                    var processor = new SpreadsheetProcessor(spreadsheet);
                    var result = processor.Evaluate(new ParallelProcessingStrategy());

                    Console.WriteLine(isConsoleOutput ? Resources.ResultTable : Resources.WritingResult);
                    using (var write = new SpreadsheetWriter(output))
                    {
                        write.WriteSpreedsheat(result);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            if (isConsoleOutput)
            {
                Console.Write(Resources.PressAnyKeyForExit);
                Console.ReadKey();
            }
        }
    }
}
