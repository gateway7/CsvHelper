namespace CsvHelper.Tests.TypeConversion
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using CsvHelper.Configuration;
    using Xunit;

    public class IEnumerableGenericConverterTests
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
                var records = csv.GetRecords<Test>().ToList();

                var list = records[0].List.ToList();

                Assert.Equal(3, list.Count);
                Assert.Equal(2, list[0]);
                Assert.Equal(3, list[1]);
                Assert.Equal(4, list[2]);
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
                writer.WriteLine("Before,List,List,List,After");
                writer.WriteLine("1,2,3,4,5");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<TestIndexMap>();
                var records = csv.GetRecords<Test>().ToList();

                var list = records[0].List.ToList();

                Assert.Equal(3, list.Count);
                Assert.Equal(2, list[0]);
                Assert.Equal(3, list[1]);
                Assert.Equal(4, list[2]);
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
                writer.WriteLine("Before,List,List,List,After");
                writer.WriteLine("1,2,3,4,5");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<TestDefaultMap>();
                var records = csv.GetRecords<Test>().ToList();

                var list = records[0].List.ToList();

                Assert.Equal(3, list.Count);
                Assert.Equal(2, list[0]);
                Assert.Equal(3, list[1]);
                Assert.Equal(4, list[2]);
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
                writer.WriteLine("Before,List,List,List,After");
                writer.WriteLine("1,2,3,4,5");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<TestNamedMap>();
                var records = csv.GetRecords<Test>().ToList();

                var list = records[0].List.ToList();

                Assert.Equal(3, list.Count);
                Assert.Equal(2, list[0]);
                Assert.Equal(3, list[1]);
                Assert.Equal(4, list[2]);
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
                writer.WriteLine("Before,List,A,List,B,List,After");
                writer.WriteLine("1,2,3,4,5,6,7");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.RegisterClassMap<TestNamedMap>();
                var records = csv.GetRecords<Test>().ToList();

                var list = records[0].List.ToList();

                Assert.Equal(3, list.Count);
                Assert.Equal(2, list[0]);
                Assert.Equal(4, list[1]);
                Assert.Equal(6, list[2]);
            }
        }

        [Fact]
        public void FullWriteNoHeaderTest()
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

                Assert.Equal(",1,2,3,\r\n", result);
            }
        }

        [Fact]
        public void FullWriteWithHeaderTest()
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
                csv.Configuration.RegisterClassMap<TestIndexMap>();
                csv.WriteRecords(list);
                writer.Flush();
                stream.Position = 0;

                var result = reader.ReadToEnd();
                var expected = new StringBuilder();
                expected.AppendLine("Before,List1,List2,List3,After");
                expected.AppendLine(",1,2,3,");

                Assert.Equal(expected.ToString(), result);
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
                Assert.Equal(null, list[0]);
                Assert.Equal(null, list[1]);
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
                Assert.Equal(null, list[0]);
                Assert.Equal(null, list[1]);
                Assert.Equal(4, list[2]);
            }
        }

        private class Test
        {
            public string Before { get; set; }

            public IEnumerable<int?> List { get; set; }

            public string After { get; set; }
        }

        private sealed class TestIndexMap : CsvClassMap<Test>
        {
            public TestIndexMap()
            {
                Map(m => m.Before).Index(0);
                Map(m => m.List).Index(1, 3);
                Map(m => m.After).Index(4);
            }
        }

        private sealed class TestNamedMap : CsvClassMap<Test>
        {
            public TestNamedMap()
            {
                Map(m => m.Before).Name("Before");
                Map(m => m.List).Name("List");
                Map(m => m.After).Name("After");
            }
        }

        private sealed class TestDefaultMap : CsvClassMap<Test>
        {
            public TestDefaultMap()
            {
                Map(m => m.Before);
                Map(m => m.List);
                Map(m => m.After);
            }
        }
    }
}