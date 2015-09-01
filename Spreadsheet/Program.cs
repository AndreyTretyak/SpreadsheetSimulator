﻿using System;
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
            try
            {
                var input = args.Length > 0 ? File.Open(args[0], FileMode.Open) : Console.OpenStandardInput();
                var output = args.Length > 1 ? File.Create(args[1]) : Console.OpenStandardOutput();
                using (var reader = new SpreadsheetReader(input))
                {
                    var spreadsheet = reader.ReadSpreadsheet();
                    var processor = new SpreadsheetProcessor(spreadsheet);
                    var result = processor.Evaluate(new ParallelProcessingStrategy());
                    using (var write = new SpreedsheatWriter(output))
                    {
                        write.WriteSpreedsheat(result);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            if (args.Length < 2)
            {
                Console.ReadKey();
            }
        }
    }
}
