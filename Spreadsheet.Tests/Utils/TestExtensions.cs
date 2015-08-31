using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Spreadsheet.Core;
using Spreadsheet.Core.Cells;
using Spreadsheet.Core.Cells.Expressions;

namespace Spreadsheet.Tests.Utils
{
    internal static class TestExtensions
    {
        public static T Cast<T>(object result)
        {
            Assert.IsInstanceOf(typeof(T), result);
            return (T)result;
        }

        public static SpreadsheetProcessor CreateProcessor(params IExpression[] expressions)
        {

            return new SpreadsheetProcessor(new Core.Spreadsheet(1, expressions.Length,
                                            expressions.Select((e, i) => new Cell(new CellAddress(0, i), e))));
        }
    }
}
