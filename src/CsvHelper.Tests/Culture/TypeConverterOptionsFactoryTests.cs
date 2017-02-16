namespace CsvHelper.Tests.Culture
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;
    using Xunit;

    public class TypeConverterOptionsFactoryTests
    {
        [Fact]
        public void AddGetRemoveTest()
        {
            var config = new CsvConfiguration();
            var customOptions = new TypeConverterOptions
            {
                Format = "custom"
            };
            config.TypeConverterOptions.Add<string>(customOptions);
            var options = config.TypeConverterOptions.Get<string>();

            Assert.Equal(customOptions.Format, options.Format);

            config.TypeConverterOptions.Remove<string>();

            options = config.TypeConverterOptions.Get<string>();

            Assert.NotEqual(customOptions.Format, options.Format);
        }

        [Fact]
        public void GetFieldTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csvReader = new CsvReader(reader))
            {
                writer.WriteLine("\"1,234\",\"5,678\"");
                writer.Flush();
                stream.Position = 0;

                var options = new TypeConverterOptions { NumberStyle = NumberStyles.AllowThousands };
                csvReader.Configuration.TypeConverterOptions.Add<int>(options);
                csvReader.Configuration.HasHeaderRecord = false;
                csvReader.Read();
                Assert.Equal(1234, csvReader.GetField<int>(0));
                Assert.Equal(5678, csvReader.GetField(typeof(int), 1));
            }
        }

        [Fact]
        public void GetFieldSwitchCulturesTest()
        {
            GetFieldForCultureTest("\"1234,32\",\"5678,44\"", "fr-FR", 1234.32M, 5678.44M);
            GetFieldForCultureTest("\"9876.54\",\"3210.98\"", "en-GB", 9876.54M, 3210.98M);
            GetFieldForCultureTest("\"4455,6677\",\"9988,77\"", "el-GR", 4455.6677M, 9988.77M);
        }

        [Fact]
        public void GetRecordsTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csvReader = new CsvReader(reader))
            {
                writer.WriteLine("\"1,234\",\"5,678\"");
                writer.Flush();
                stream.Position = 0;

                var options = new TypeConverterOptions { NumberStyle = NumberStyles.AllowThousands };
                csvReader.Configuration.TypeConverterOptions.Add<int>(options);
                csvReader.Configuration.HasHeaderRecord = false;
                csvReader.GetRecords<Test>().ToList();
            }
        }

        [Fact]
        public void GetRecordsAppliedWhenMappedTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csvReader = new CsvReader(reader))
            {
                writer.WriteLine("\"1,234\",\"$5,678\"");
                writer.Flush();
                stream.Position = 0;

                var options = new TypeConverterOptions { NumberStyle = NumberStyles.AllowThousands };
                csvReader.Configuration.TypeConverterOptions.Add<int>(options);
                csvReader.Configuration.HasHeaderRecord = false;
                csvReader.Configuration.RegisterClassMap<TestMap>();
                csvReader.GetRecords<Test>().ToList();
            }
        }

        [Fact]
        public void WriteFieldTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csvWriter = new CsvWriter(writer))
            {
                var options = new TypeConverterOptions { Format = "c" };
                csvWriter.Configuration.TypeConverterOptions.Add<int>(options);
                csvWriter.WriteField(1234);
                csvWriter.NextRecord();
                writer.Flush();
                stream.Position = 0;
                var record = reader.ReadToEnd();

                Assert.Equal("\"$1,234.00\"\r\n", record);
            }
        }

        [Fact]
        public void WriteRecordsTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csvWriter = new CsvWriter(writer))
            {
                var list = new List<Test>
                {
                    new Test { Number = 1234, NumberOverriddenInMap = 5678 }
                };
                var options = new TypeConverterOptions { Format = "c" };
                csvWriter.Configuration.TypeConverterOptions.Add<int>(options);
                csvWriter.Configuration.HasHeaderRecord = false;
                csvWriter.WriteRecords(list);
                writer.Flush();
                stream.Position = 0;
                var record = reader.ReadToEnd();

                Assert.Equal("\"$1,234.00\",\"$5,678.00\"\r\n", record);
            }
        }

        [Fact]
        public void WriteRecordsAppliedWhenMappedTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csvWriter = new CsvWriter(writer))
            {
                var list = new List<Test>
                {
                    new Test { Number = 1234, NumberOverriddenInMap = 5678 }
                };
                var options = new TypeConverterOptions { Format = "c" };
                csvWriter.Configuration.TypeConverterOptions.Add<int>(options);
                csvWriter.Configuration.HasHeaderRecord = false;
                csvWriter.Configuration.RegisterClassMap<TestMap>();
                csvWriter.WriteRecords(list);
                writer.Flush();
                stream.Position = 0;
                var record = reader.ReadToEnd();

                Assert.Equal("\"$1,234.00\",\"5,678.00\"\r\n", record);
            }
        }

        private static void GetFieldForCultureTest(string csvText, string culture, decimal expected1, decimal expected2)
        {
            using (var reader = new StringReader(csvText))
            using (var csvReader = new CsvReader(reader, new CsvConfiguration { CultureInfo = new CultureInfo(culture) }))
            {
                csvReader.Configuration.HasHeaderRecord = false;
                csvReader.Read();
                Assert.Equal(expected1, csvReader.GetField<decimal>(0));
                Assert.Equal(expected2, csvReader.GetField(typeof(decimal), 1));
            }
        }

        private class Test
        {
            public int Number { get; set; }

            public int NumberOverriddenInMap { get; set; }
        }

        private sealed class TestMap : CsvClassMap<Test>
        {
            public TestMap()
            {
                Map(m => m.Number);
                Map(m => m.NumberOverriddenInMap).TypeConverterOption.NumberStyles(NumberStyles.AllowThousands | NumberStyles.AllowCurrencySymbol).TypeConverterOption.Format("N");
            }
        }
    }
}