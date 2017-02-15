namespace CsvHelper.Tests.Reading
{
    using System.IO;
    using Xunit;

    public class EmptyTextReaderTests
    {
        [Fact]
        public void EmptyStreamDoesntFailTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                Assert.False(csv.Read());
            }
        }
    }
}