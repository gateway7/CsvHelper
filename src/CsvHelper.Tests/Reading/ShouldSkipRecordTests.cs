namespace CsvHelper.Tests.Reading
{
    using System.Collections.Generic;
    using System.Linq;
    using Mocks;
    using Xunit;

    public class ShouldSkipRecordTests
    {
        [Fact]
        public void SkipEmptyHeaderTest()
        {
            var rows = new Queue<string[]>();
            rows.Enqueue(new[] { " " });
            rows.Enqueue(new[] { "First,Second" });
            rows.Enqueue(new[] { "1", "2" });
            var parser = new ParserMock(rows);

            var csv = new CsvReader(parser);
            csv.Configuration.ShouldSkipRecord = row => row.All(string.IsNullOrWhiteSpace);

            csv.Read();
            csv.ReadHeader();
            csv.Read();
            Assert.Equal("1", csv.GetField(0));
            Assert.Equal("2", csv.GetField(1));
        }

        [Fact]
        public void SkipEmptyRowTest()
        {
            var rows = new Queue<string[]>();
            rows.Enqueue(new[] { "First,Second" });
            rows.Enqueue(new[] { " " });
            rows.Enqueue(new[] { "1", "2" });
            var parser = new ParserMock(rows);

            var csv = new CsvReader(parser);
            csv.Configuration.ShouldSkipRecord = row => row.All(string.IsNullOrWhiteSpace);

            csv.Read();
            csv.ReadHeader();
            csv.Read();
            Assert.Equal("1", csv.GetField(0));
            Assert.Equal("2", csv.GetField(1));
        }

        [Fact]
        public void ShouldSkipWithEmptyRows()
        {
            var rows = new Queue<string[]>();
            rows.Enqueue(new[] { "First,Second" });
            rows.Enqueue(new[] { "skipme," });
            rows.Enqueue(new[] { "" });
            rows.Enqueue(new[] { "1", "2" });

            var parser = new ParserMock(rows);

            var csv = new CsvReader(parser);
            csv.Configuration.ShouldSkipRecord = row => row[0].StartsWith("skipme");
            csv.Configuration.SkipEmptyRecords = true;

            csv.Read();
            csv.Read();
            Assert.Equal("1", csv.GetField(0));
            Assert.Equal("2", csv.GetField(1));
        }
    }
}