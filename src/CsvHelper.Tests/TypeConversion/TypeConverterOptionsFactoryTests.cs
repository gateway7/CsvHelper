﻿// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.Tests.TypeConversion
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
            var customOptions = new TypeConverterOptions
            {
                Format = "custom"
            };
            var typeConverterOptionsFactory = new TypeConverterOptionsCollection();

            typeConverterOptionsFactory.Add<string>(customOptions);
            var options = typeConverterOptionsFactory.Get<string>();

            Assert.Equal(customOptions.Format, options.Format);

            typeConverterOptionsFactory.Remove<string>();

            options = typeConverterOptionsFactory.Get<string>();

            Assert.NotEqual(customOptions.Format, options.Format);
        }

        [Fact]
        public void GetFieldTest()
        {
            var options = new TypeConverterOptions { NumberStyle = NumberStyles.AllowThousands };

            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csvReader = new CsvReader(reader))
            {
                writer.WriteLine("\"1,234\",\"5,678\"");
                writer.Flush();
                stream.Position = 0;

                csvReader.Configuration.TypeConverterOptions.Add<int>(options);
                csvReader.Configuration.HasHeaderRecord = false;
                csvReader.Read();
                Assert.Equal(1234, csvReader.GetField<int>(0));
                Assert.Equal(5678, csvReader.GetField(typeof(int), 1));
            }
        }

        [Fact]
        public void GetRecordsTest()
        {
            var options = new TypeConverterOptions { NumberStyle = NumberStyles.AllowThousands };

            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csvReader = new CsvReader(reader))
            {
                writer.WriteLine("\"1,234\",\"5,678\"");
                writer.Flush();
                stream.Position = 0;

                csvReader.Configuration.TypeConverterOptions.Add<int>(options);
                csvReader.Configuration.HasHeaderRecord = false;
                csvReader.GetRecords<Test>().ToList();
            }
        }

        [Fact]
        public void GetRecordsAppliedWhenMappedTest()
        {
            var options = new TypeConverterOptions { NumberStyle = NumberStyles.AllowThousands };

            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csvReader = new CsvReader(reader))
            {
                writer.WriteLine("\"1,234\",\"$5,678\"");
                writer.Flush();
                stream.Position = 0;

                csvReader.Configuration.TypeConverterOptions.Add<int>(options);
                csvReader.Configuration.HasHeaderRecord = false;
                csvReader.Configuration.RegisterClassMap<TestMap>();
                csvReader.GetRecords<Test>().ToList();
            }
        }

        [Fact]
        public void WriteFieldTest()
        {
            var options = new TypeConverterOptions { Format = "c" };

            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csvWriter = new CsvWriter(writer))
            {
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
            var options = new TypeConverterOptions { Format = "c" };

            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csvWriter = new CsvWriter(writer))
            {
                var list = new List<Test>
                {
                    new Test { Number = 1234, NumberOverriddenInMap = 5678 }
                };
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
            var options = new TypeConverterOptions { Format = "c" };

            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csvWriter = new CsvWriter(writer))
            {
                var list = new List<Test>
                {
                    new Test { Number = 1234, NumberOverriddenInMap = 5678 }
                };
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