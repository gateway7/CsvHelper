
#if WINRT_4_5
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else

#endif

namespace CsvHelper.Tests.Reading
{
    using System.Collections.Generic;
    using Mocks;
    using Xunit;

    public class ReadHeaderTests
    {
        [Fact]
        public void ReadHeaderReadsHeaderTest()
        {
            var rows = new Queue<string[]>();
            rows.Enqueue(new[] { "Id", "Name" });
            rows.Enqueue(new[] { "1", "One" });
            rows.Enqueue(new[] { "2", "two" });
            rows.Enqueue(null);
            var parser = new ParserMock(rows);

            var csv = new CsvReader(parser);
            csv.Read();
            csv.ReadHeader();

            Assert.NotNull(csv.FieldHeaders);
            Assert.Equal("Id", csv.FieldHeaders[0]);
            Assert.Equal("Name", csv.FieldHeaders[1]);
        }

        [Fact]
        public void ReadHeaderDoesNotReadRowTest()
        {
            var rows = new Queue<string[]>();
            rows.Enqueue(new[] { "Id", "Name" });
            rows.Enqueue(new[] { "1", "One" });
            rows.Enqueue(new[] { "2", "two" });
            rows.Enqueue(null);
            var parser = new ParserMock(rows);

            var csv = new CsvReader(parser);
            csv.Read();
            csv.ReadHeader();

            Assert.Null(csv.CurrentRecord);
        }

        [Fact]
        public void GettingFieldHeadersFailsWhenHeaderNotReadTest()
        {
            var rows = new Queue<string[]>();
            rows.Enqueue(new[] { "Id", "Name" });
            rows.Enqueue(new[] { "1", "One" });
            rows.Enqueue(new[] { "2", "two" });
            rows.Enqueue(null);

            var parser = new ParserMock(rows);
            var csv = new CsvReader(parser);

            Assert.Throws<CsvReaderException>(() => csv.FieldHeaders);
        }

        [Fact]
        public void ReadingHeaderFailsWhenReaderIsDoneTest()
        {
            var rows = new Queue<string[]>();
            rows.Enqueue(new[] { "Id", "Name" });
            rows.Enqueue(new[] { "1", "One" });
            rows.Enqueue(new[] { "2", "two" });
            rows.Enqueue(null);
            var parser = new ParserMock(rows);

            var csv = new CsvReader(parser);
            csv.Configuration.HasHeaderRecord = false;

            while (csv.Read()) {}

            Assert.Throws<CsvReaderException>(() => csv.ReadHeader());
        }

        [Fact]
        public void ReadingHeaderFailsWhenNoHeaderRecordTest()
        {
            var rows = new Queue<string[]>();
            rows.Enqueue(new[] { "Id", "Name" });
            rows.Enqueue(new[] { "1", "One" });
            rows.Enqueue(new[] { "2", "two" });
            rows.Enqueue(null);
            var parser = new ParserMock(rows);

            var csv = new CsvReader(parser);
            csv.Configuration.HasHeaderRecord = false;

            Assert.Throws<CsvReaderException>(() => csv.ReadHeader());
        }

        [Fact]
        public void ReadingHeaderFailsWhenHeaderAlreadyReadTest()
        {
            var rows = new Queue<string[]>();
            rows.Enqueue(new[] { "Id", "Name" });
            rows.Enqueue(new[] { "1", "One" });
            rows.Enqueue(new[] { "2", "two" });
            rows.Enqueue(null);
            var parser = new ParserMock(rows);

            var csv = new CsvReader(parser);
            csv.Read();
            csv.ReadHeader();

            Assert.Throws<CsvReaderException>(() => csv.ReadHeader());
        }
    }
}