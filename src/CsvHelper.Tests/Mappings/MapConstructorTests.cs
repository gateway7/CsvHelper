namespace CsvHelper.Tests.Mappings
{
    using System;
    using System.IO;
    using CsvHelper.Configuration;
    using Xunit;

    public class MapConstructorTests
    {
        [Fact]
        public void NoConstructor()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                try
                {
                    csv.Configuration.RegisterClassMap<TestMap>();
                    Assert.True(false);
                }
                catch (InvalidOperationException ex)
                {
                    Assert.Equal("No public parameterless constructor found.", ex.Message);
                }
            }
        }

        private class Test
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        private sealed class TestMap : CsvClassMap<Test>
        {
            private TestMap()
            {
                Map(m => m.Id);
                Map(m => m.Name);
            }
        }
    }
}