namespace CsvHelper.Tests.Reading
{
    using System.Collections.Generic;
    using System.Linq;
    using CsvHelper.Configuration;
    using Mocks;
    using Xunit;

    public class ConstantTests
    {
        [Fact]
        public void ConstantAlwaysReturnsSameValueTest()
        {
            var rows = new Queue<string[]>();
            rows.Enqueue(new[] { "Id", "Name" });
            rows.Enqueue(new[] { "1", "one" });
            rows.Enqueue(new[] { "2", "two" });
            rows.Enqueue(null);
            var parser = new ParserMock(rows);

            var csv = new CsvReader(parser);
            csv.Configuration.RegisterClassMap<TestMap>();
            var records = csv.GetRecords<Test>().ToList();

            Assert.Equal(1, records[0].Id);
            Assert.Equal("constant", records[0].Name);
            Assert.Equal(2, records[1].Id);
            Assert.Equal("constant", records[0].Name);
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
                Map(m => m.Id);
                Map(m => m.Name).Constant("constant");
            }
        }
    }
}