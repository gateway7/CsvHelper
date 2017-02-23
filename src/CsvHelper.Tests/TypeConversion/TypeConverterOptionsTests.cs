namespace CsvHelper.Tests.TypeConversion
{
    using System.IO;
    using System.Linq;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;
    using Xunit;

    public class TypeConverterOptionsTests
    {
        [Fact]
        public void GlobalNullValueTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine(",");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.TypeConverterOptions.Get<string>().NullValues.Add(string.Empty);
                var records = csv.GetRecords<Test>().ToList();

                Assert.Null(records[0].Id);
                Assert.Null(records[0].Name);
            }
        }

        [Fact]
        public void TreatNullAsDefaultTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("NULL,null,0,,null");
                writer.Flush();

                stream.Position = 0;
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.TranslateLiteralNulls = false;
                csv.Configuration.TypeConverterOptions.Default.TreatNullAsDefault = false;

                Assert.Throws<CsvTypeConverterException>(() => csv.GetRecords<NullTest>().ToList());

                stream.Position = 0;
                csv.Configuration.TypeConverterOptions.Clear();
                csv.Configuration.TranslateLiteralNulls = true;
                csv.Configuration.TypeConverterOptions.Default.TreatNullAsDefault = true;

                var records = csv.GetRecords<NullTest>().ToList();

                Assert.Equal(0, records[0].Int8);
                Assert.Equal(0, records[0].Int16);
                Assert.Equal(0, records[0].Int32);
                Assert.Equal(0, records[0].Int64);
                Assert.Equal(Num.Zero, records[0].Num);
            }
        }

        [Fact]
        public void MappingNullValueTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine(",");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.RegisterClassMap<TestMap>();
                var records = csv.GetRecords<Test>().ToList();

                Assert.Null(records[0].Id);
                Assert.Null(records[0].Name);
            }
        }

        [Fact]
        public void GlobalAndMappingNullValueTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine(",");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.TypeConverterOptions.Get<string>().NullValues.Add("null");
                csv.Configuration.RegisterClassMap<TestMap>();
                var records = csv.GetRecords<Test>().ToList();

                Assert.Null(records[0].Id);
                Assert.Null(records[0].Name);
            }
        }

        private class Test
        {
            public int? Id { get; set; }

            public string Name { get; set; }
        }

        private enum Num
        {
            Zero, One, Two
        }

        private class NullTest
        {
            public byte Int8 { get; set; }

            public short Int16 { get; set; }

            public int Int32 { get; set; }

            public long Int64 { get; set; }

            public Num Num { get; set; }
        }

        private sealed class TestMap : CsvClassMap<Test>
        {
            public TestMap()
            {
                Map(m => m.Id);
                Map(m => m.Name).TypeConverterOption.NullValues(string.Empty);
            }
        }

        // auto map options have defaults
        // map options could be default or custom if set
        // global has defaults or custom

        // merge global with map
    }
}