using System;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Spreadsheet.Core;

namespace Spreadsheet.Tests;

[TestFixture]
public class SpreadsheetWriterTests
{
    public string GetResult(int columnCount, params object[] values)
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = new SpreadsheetWriter(stream))
            {
                writer.WriteSpreadsheet(new SpreadsheetProcessingResult(columnCount, values));
                stream.Position = 0;
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

    private object[] GetValues(string result)
    {
        var values = result.Split(new[] { "\t\r\n", "\t" }, StringSplitOptions.None);
        return values.Take(values.Length - 1).Cast<object>().ToArray();
    }


    [Test]
    [TestCase(1, "1\t\r\n@\t\r\n123\t\r\n")]
    [TestCase(3, "1\t@\t123\t\r\n")]
    [TestCase(2, "1\t@\t\r\n123\t0\t\r\n")]
    [TestCase(2, "\t\t\r\n\t\t\r\n")]
    [TestCase(1, "\t\r\n")]
    public void StringValuesTest(int columnCount, string expect)
    {
        Assert.AreEqual(expect, GetResult(columnCount, GetValues(expect)));
    }

    [Test]
    [TestCase(1, 3, "@", 123, "2")]
    [TestCase(2, 9, "\0", "", "91")]
    [TestCase(4, 9123465, "test + text", 0, "12")]
    [TestCase(1, new object[] { 12312312312321312321 })]
    [TestCase(1, new object[] { "" })]
    public void ObjectValuesTest(int columnCount, params object[] values)
    {
        CollectionAssert.AreEqual(values.Select(e => e?.ToString()), GetValues(GetResult(columnCount, values)));
    }
}
