// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com


#if WINRT_4_5
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else

#endif

namespace CsvHelper.Tests
{
    using System.IO;
    using System.Text;
    using CsvHelper.Configuration;
    using Xunit;

    public class ClearRecordsCacheTests
    {
        [Fact]
        public void ClearReaderTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("Id,Name");
                writer.WriteLine("1,one");
                writer.WriteLine("2,two");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.RegisterClassMap<TestMap1>();
                csv.Read();
                var record = csv.GetRecord<Test>();

                Assert.NotNull(record);
                Assert.Equal(1, record.Id);
                Assert.Equal(null, record.Name);

                stream.Position = 0;
                csv.ClearRecordCache<Test>();

                csv.Configuration.RegisterClassMap<TestMap2>();
                csv.Read();
                record = csv.GetRecord<Test>();

                Assert.NotNull(record);
                Assert.Equal(0, record.Id);
                Assert.Equal("two", record.Name);
            }
        }

        [Fact]
        public void ClearWriterTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.RegisterClassMap<TestMap1>();
                var record = new Test { Id = 1, Name = "one" };
                csv.WriteRecord(record);
                csv.NextRecord();

                csv.ClearRecordCache<Test>();
                csv.Configuration.RegisterClassMap<TestMap2>();
                record = new Test { Id = 2, Name = "two" };
                csv.WriteRecord(record);
                csv.NextRecord();

                writer.Flush();
                stream.Position = 0;

                var data = reader.ReadToEnd();

                var expected = new StringBuilder();
                expected.AppendLine("1");
                expected.AppendLine("two");

                Assert.Equal(expected.ToString(), data);
            }
        }

        private class Test
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        private sealed class TestMap1 : CsvClassMap<Test>
        {
            public TestMap1()
            {
                Map(m => m.Id);
            }
        }

        private sealed class TestMap2 : CsvClassMap<Test>
        {
            public TestMap2()
            {
                Map(m => m.Name);
            }
        }
    }
}