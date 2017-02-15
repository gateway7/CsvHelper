namespace CsvHelper.Tests.Writing
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using CsvHelper.Configuration;
    using Xunit;

    public class NoPropertyMappingTests
    {
        [Fact]
        public void NoPropertyWithHeaderAndNameTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                var list = new List<Test>
                {
                    new Test { Id = 1 },
                    new Test { Id = 2 }
                };

                csv.Configuration.RegisterClassMap<TestWithNameMap>();
                csv.WriteRecords(list);

                writer.Flush();
                stream.Position = 0;

                var result = reader.ReadToEnd();

                var expected = new StringBuilder();
                expected.AppendLine("Id,Constant");
                expected.AppendLine("1,const");
                expected.AppendLine("2,const");

                Assert.Equal(expected.ToString(), result);
            }
        }

        [Fact]
        public void NoPropertyWithHeaderAndNoNameTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                var list = new List<Test>
                {
                    new Test { Id = 1 },
                    new Test { Id = 2 }
                };

                csv.Configuration.RegisterClassMap<TestWithNoNameMap>();
                csv.WriteRecords(list);

                writer.Flush();
                stream.Position = 0;

                var result = reader.ReadToEnd();

                var expected = new StringBuilder();
                expected.AppendLine("Id,");
                expected.AppendLine("1,const");
                expected.AppendLine("2,const");

                Assert.Equal(expected.ToString(), result);
            }
        }

        [Fact]
        public void NoPropertyWithNoHeaderAndNameTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                var list = new List<Test>
                {
                    new Test { Id = 1 },
                    new Test { Id = 2 }
                };

                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.RegisterClassMap<TestWithNameMap>();
                csv.WriteRecords(list);

                writer.Flush();
                stream.Position = 0;

                var result = reader.ReadToEnd();

                var expected = new StringBuilder();
                expected.AppendLine("1,const");
                expected.AppendLine("2,const");

                Assert.Equal(expected.ToString(), result);
            }
        }

        [Fact]
        public void NoPropertyWithNoHeaderAndNoNameTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                var list = new List<Test>
                {
                    new Test { Id = 1 },
                    new Test { Id = 2 }
                };

                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.RegisterClassMap<TestWithNoNameMap>();
                csv.WriteRecords(list);

                writer.Flush();
                stream.Position = 0;

                var result = reader.ReadToEnd();

                var expected = new StringBuilder();
                expected.AppendLine("1,const");
                expected.AppendLine("2,const");

                Assert.Equal(expected.ToString(), result);
            }
        }

        private class Test
        {
            public int Id { get; set; }
        }

        private sealed class TestWithNameMap : CsvClassMap<Test>
        {
            public TestWithNameMap()
            {
                Map(m => m.Id);
                Map().Name("Constant").Constant("const");
            }
        }

        private sealed class TestWithNoNameMap : CsvClassMap<Test>
        {
            public TestWithNoNameMap()
            {
                Map(m => m.Id);
                Map().Constant("const");
            }
        }
    }
}