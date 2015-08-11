using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using SpreadsheetProcessor.Cells;

namespace SpreadsheetProcessor
{
    public class CellAdress
    {
        public int Row { get; }

        public int Column { get; }

        public string StringValue => $"R{Row}C{Column}";

        public CellAdress(int row, int column)
        {
            if (Row < 0)
                throw new IndexOutOfRangeException("Cell row can`t be negative");
            if (Column < 0)
                throw new IndexOutOfRangeException("Cell column can`t be negative");
            Row = row;
            Column = column;
        }

        public void Validate(CellAdress maxPosible)
        {
            if (maxPosible.Row > Row)
                throw new IndexOutOfRangeException("Cell row is more then table size");
            if (maxPosible.Column > Column)
                throw new IndexOutOfRangeException("Cell column is more then table size");
        }
    }

    public class SpreadsheetSource
    {
        public CellAdress MaxAdress { get; }

        private string[][] Content { get; }

        public SpreadsheetSource(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                //TODO: need validation
                var size = reader.ReadLine().Split('\t').ToArray();
                MaxAdress = new CellAdress(int.Parse(size[0]), int.Parse(size[1]));
                Content = reader.ReadToEnd().Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                                .Select(e => e.Split(new [] {'\t'}, StringSplitOptions.RemoveEmptyEntries).ToArray())
                                .ToArray();
            }
        }

        public string GetCellContent(CellAdress cellAdress)
        {
            cellAdress.Validate(MaxAdress);
            return Content[cellAdress.Row][cellAdress.Column];
        }
    }

    public class Spreadsheet
    {
        
        private SpreadsheetSource Source { get; }

        public Spreadsheet(SpreadsheetSource source)
        {
            Source = source;
        }

        public Cell GetCell(CellAdress cellAdress)
        {
            var value = Source.GetCellContent(cellAdress);

            return null;
        }
    }

    public class ParserSettings
    {
        public const char StringStart = '\'';

        public const char ExpressionStart = '=';

        public const string AdditionOperator = "+";

        public const string SubtractionOperator = "-";

        public const string MultiplicationOperator = "*";

        public const string DivisionOperator = "/";

        public static string[] Operators => new [] {AdditionOperator, SubtractionOperator, MultiplicationOperator, DivisionOperator};

        public const string CallStackSeparator = "|";
    }

    public class ExpressionParser
    {
        public IExpression Parse(string expresion)
        {
            if (string.IsNullOrWhiteSpace(expresion))
                return new ConstantExpression(new ExpressionValue(CellValueType.Nothing, null));

            var firstChar = expresion.First();

            if (char.IsDigit(firstChar))
                return ParseNumber(expresion);

            switch (firstChar)
            {
                case ParserSettings.StringStart:
                    return ParseString(expresion);
                case ParserSettings.ExpressionStart:
                    return ParseExpression(expresion);
            }

            return new ConstantExpression(new ExpressionValue(CellValueType.Error, string.Format(Resources.InvalidCellContent, expresion)));
        }

        public IExpression ParseNumber(string expresion)
        {
            int result;
            return int.TryParse(expresion, out result) 
                ? new ConstantExpression(new ExpressionValue(CellValueType.Integer,  result)) 
                : new ConstantExpression(new ExpressionValue(CellValueType.Error, string.Format(Resources.FailedToParseNumber, expresion)));
        }

        public IExpression ParseString(string expresion)
        {
            return new ConstantExpression(new ExpressionValue(CellValueType.String, expresion.Substring(1)));
        }

        public IExpression ParseExpression(string expresion)
        {
            throw new NotImplementedException();
        }


    }

    public class SpreedsheetProcessor
    {
        private Spreadsheet Spreadsheet { get;  }

        public SpreedsheetProcessor(Spreadsheet spreadsheet)
        {
            Spreadsheet = spreadsheet;
        }

        public ExpressionValue GetCellValue(CellAdress cellAdress)
        {
            return GetCellValue(cellAdress, null);
        }

        internal ExpressionValue GetCellValue(CellAdress cellAdress, string callStack)
        {
            var value = Spreadsheet.GetCell(cellAdress);
            return value.Evaluate(this, callStack);
        }
    }
}
