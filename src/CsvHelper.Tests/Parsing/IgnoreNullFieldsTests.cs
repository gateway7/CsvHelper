namespace CsvHelper.Tests.Parsing
{
    using System.IO;
    using System.Linq;
    using Xunit;

    public class IgnoreNullFieldsTests
    {
        [Fact]
        public void IgnoreNullFieldsTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.WriteLine("1,null,3,NULL");
                writer.Flush();

                stream.Position = 0;
                parser.Configuration.IgnoreNullFields = false;
                var row = parser.Read();

                Assert.True(row.SequenceEqual(new[] { "1", "null", "3", "NULL" }));

                stream.Position = 0;
                parser.Configuration.IgnoreNullFields = true;
                row = parser.Read();

                Assert.True(row.SequenceEqual(new[] { "1", null, "3", null }));
            }
        }
    }
}