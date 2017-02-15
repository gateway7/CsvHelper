namespace CsvHelper.Tests.Mappings
{
    using System.IO;
    using System.Linq;
    using CsvHelper.Configuration;
    using Xunit;

    public class IgnoreHeaderWhiteSpaceTests
    {
        [Fact]
        public void Blah()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("The Id,The Name");
                writer.WriteLine("1,one");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.RegisterClassMap<TestMap>();
                var records = csv.GetRecords<Test>().ToList();

                Assert.Equal(1, records[0].Id);
                Assert.Equal("one", records[0].Name);
            }
        }

        private class Test
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        private sealed class TestMap : CsvClassMap<Test>
        {
            public TestMap()
            {
                Map(m => m.Id).Name("The Id");
                Map(m => m.Name).Name("The Name");
            }
        }
    }
}