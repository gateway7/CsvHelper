namespace CsvHelper.Tests.Reading
{
    using System.IO;
    using System.Linq;
    using Xunit;

    public class MultipleGetRecordsTests
    {
        [Fact]
        public void Blah()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("Id,Name");
                writer.WriteLine("1,one");
                writer.Flush();
                stream.Position = 0;

                var records = csv.GetRecords<Test>().ToList();

                var position = stream.Position;
                writer.WriteLine("2,two");
                writer.Flush();
                stream.Position = position;

                records = csv.GetRecords<Test>().ToList();

                writer.WriteLine("2,two");
                writer.Flush();
                stream.Position = position;

                Assert.Equal(1, records.Count);
                Assert.Equal(2, records[0].Id);
                Assert.Equal("two", records[0].Name);
            }
        }

        private class Test
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}