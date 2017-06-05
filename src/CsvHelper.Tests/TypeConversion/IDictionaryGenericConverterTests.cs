namespace CsvHelper.Tests.TypeConversion
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CsvHelper.Configuration;
    using Xunit;

    // ReSharper disable once InconsistentNaming
    public class IDictionaryGenericConverterTests
    {
        [Fact]
        public void FullReadNoHeaderTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("1,2,3,4,5");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.RegisterClassMap<TestIndexMap>();

                // You can't read into a dictionary without a header.
                // You need to header value to use as the key.
                Assert.Throws<CsvReaderException>(() => csv.GetRecords<Test>().ToList());
            }
        }

        [Fact]
        public void FullReadWithHeaderTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("Before,Dictionary1,Dictionary2,Dictionary3,After");
                writer.WriteLine("1,2,3,4,5");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<TestIndexMap>();
                var records = csv.GetRecords<Test>().ToList();

                var list = records[0].Dictionary;

                Assert.Equal(3, list.Count);
                Assert.Equal(2, list["Dictionary1"]);
                Assert.Equal(3, list["Dictionary2"]);
                Assert.Equal(4, list["Dictionary3"]);
            }
        }

        [Fact]
        public void FullReadWithDefaultHeaderTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("Before,Dictionary,Dictionary,Dictionary,After");
                writer.WriteLine("1,2,3,4,5");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<TestDefaultMap>();

                // Can't have same name with Dictionary
                Assert.Throws<CsvReaderException>(() => csv.GetRecords<Test>().ToList());
            }
        }

        [Fact]
        public void FullReadWithNamedHeaderTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("Before,Dictionary,Dictionary,Dictionary,After");
                writer.WriteLine("1,2,3,4,5");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<TestNamedMap>();

                // Can't have same name with Dictionary.
                Assert.Throws<CsvReaderException>(() => csv.GetRecords<Test>().ToList());
            }
        }

        [Fact]
        public void FullReadWithHeaderListItemsScattered()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("Before,Dictionary,A,Dictionary,B,Dictionary,After");
                writer.WriteLine("1,2,3,4,5,6,7");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<TestNamedMap>();

                // Can't have same name with Dictionary.
                Assert.Throws<CsvReaderException>(() => csv.GetRecords<Test>().ToList());
            }
        }

        [Fact]
        public void ReadNullValuesIndexTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("Before,D1,D2,D3,After");
                writer.WriteLine("1,null,NULL,4,5");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<TestIndexMap>();
                var records = csv.GetRecords<Test>().ToList();
                var list = records[0].Dictionary;

                Assert.Equal(3, list.Count);
                Assert.Null(list["D1"]);
                Assert.Null(list["D2"]);
                Assert.Equal(4, list["D3"]);
            }
        }

        private class Test
        {
            public string Before { get; set; }

            public Dictionary<string, int?> Dictionary { get; set; }

            public string After { get; set; }
        }

        private sealed class TestIndexMap : CsvClassMap<Test>
        {
            public TestIndexMap()
            {
                Map(m => m.Before).Index(0);
                Map(m => m.Dictionary).Index(1, 3);
                Map(m => m.After).Index(4);
            }
        }

        private sealed class TestNamedMap : CsvClassMap<Test>
        {
            public TestNamedMap()
            {
                Map(m => m.Before).Name("Before");
                Map(m => m.Dictionary).Name("Dictionary");
                Map(m => m.After).Name("After");
            }
        }

        private sealed class TestDefaultMap : CsvClassMap<Test>
        {
            public TestDefaultMap()
            {
                Map(m => m.Before);
                Map(m => m.Dictionary);
                Map(m => m.After);
            }
        }
    }
}