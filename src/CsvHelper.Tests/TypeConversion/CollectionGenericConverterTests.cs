namespace CsvHelper.Tests.TypeConversion
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CsvHelper.Configuration;
    using Xunit;

    public class CollectionGenericConverterTests
    {
        [Fact]
        public void FullWriteTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                var list = new List<Test>
                {
                    new Test { List = new List<int?> { 1, 2, 3 } }
                };
                csv.Configuration.HasHeaderRecord = false;
                csv.WriteRecords(list);
                writer.Flush();
                stream.Position = 0;

                var result = reader.ReadToEnd();

                Assert.Equal("1,2,3\r\n", result);
            }
        }

        [Fact]
        public void ReadNullValuesNameTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("Before,List,List,List,After");
                writer.WriteLine("1,null,NULL,4,5");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<TestNamedMap>();
                var records = csv.GetRecords<Test>().ToList();

                var list = records[0].List.ToList();

                Assert.Equal(3, list.Count);
                Assert.Null(list[0]);
                Assert.Null(list[1]);
                Assert.Equal(4, list[2]);
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
                writer.WriteLine("1,null,NULL,4,5");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.RegisterClassMap<TestIndexMap>();
                var records = csv.GetRecords<Test>().ToList();

                var list = records[0].List.ToList();

                Assert.Equal(3, list.Count);
                Assert.Null(list[0]);
                Assert.Null(list[1]);
                Assert.Equal(4, list[2]);
            }
        }

        private class Test
        {
            public List<int?> List { get; set; }
        }

        private sealed class TestIndexMap : CsvClassMap<Test>
        {
            public TestIndexMap()
            {
                Map(m => m.List).Index(1, 3);
            }
        }

        private sealed class TestNamedMap : CsvClassMap<Test>
        {
            public TestNamedMap()
            {
                Map(m => m.List).Name("List");
            }
        }

        private sealed class TestDefaultMap : CsvClassMap<Test>
        {
            public TestDefaultMap()
            {
                Map(m => m.List);
            }
        }
    }
}