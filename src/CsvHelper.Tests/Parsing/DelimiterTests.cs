namespace CsvHelper.Tests.Parsing
{
    using System.IO;
    using Xunit;

    public class DelimiterTests
    {
        [Fact]
        public void MultipleCharDelimiterWithPartOfDelimiterInFieldTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.Write("1&|$2&3&|$4\r\n");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.Delimiter = "&|$";
                var line = parser.Read();

                Assert.Equal(3, line.Length);
                Assert.Equal("1", line[0]);
                Assert.Equal("2&3", line[1]);
                Assert.Equal("4", line[2]);
            }
        }
    }
}