// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.Tests.ExcelCompatibility
{
    using System.Collections.Generic;
    using System.IO;
    using Xunit;

    public class ExcelCompatibleTests
    {
        [Fact]
        public void ParseTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.WriteLine("one,two,three");
                writer.Flush();
                stream.Position = 0;

                var record = parser.Read();

                Assert.NotNull(record);
                Assert.Equal(3, record.Length);
                Assert.Equal("one", record[0]);
                Assert.Equal("two", record[1]);
                Assert.Equal("three", record[2]);
            }
        }

        [Fact]
        public void ParseEscapedFieldsTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                // "one","two","three"
                writer.WriteLine("\"one\",\"two\",\"three\"");
                writer.Flush();
                stream.Position = 0;

                var record = parser.Read();

                Assert.NotNull(record);
                Assert.Equal(3, record.Length);
                Assert.Equal("one", record[0]);
                Assert.Equal("two", record[1]);
                Assert.Equal("three", record[2]);
            }
        }

        [Fact]
        public void ParseEscapedAndNonFieldsTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                // one,"two",three
                writer.WriteLine("one,\"two\",three");
                writer.Flush();
                stream.Position = 0;

                var record = parser.Read();

                Assert.NotNull(record);
                Assert.Equal(3, record.Length);
                Assert.Equal("one", record[0]);
                Assert.Equal("two", record[1]);
                Assert.Equal("three", record[2]);
            }
        }

        [Fact]
        public void ParseEscapedFieldWithSpaceAfterTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                // one,"two" ,three
                writer.WriteLine("one,\"two\" ,three");
                writer.Flush();
                stream.Position = 0;

                var record = parser.Read();

                Assert.NotNull(record);
                Assert.Equal(3, record.Length);
                Assert.Equal("one", record[0]);
                Assert.Equal("two ", record[1]);
                Assert.Equal("three", record[2]);
            }
        }

        [Fact]
        public void ParseEscapedFieldWithSpaceBeforeTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                // one, "two",three
                writer.WriteLine("one, \"two\",three");
                writer.Flush();
                stream.Position = 0;

                var record = parser.Read();

                Assert.NotNull(record);
                Assert.Equal(3, record.Length);
                Assert.Equal("one", record[0]);
                Assert.Equal(" \"two\"", record[1]);
                Assert.Equal("three", record[2]);
            }
        }

        [Fact]
        public void ParseEscapedFieldWithQuoteAfterTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                // 1,"two" "2,3
                writer.WriteLine("1,\"two\" \"2,3");
                writer.Flush();
                stream.Position = 0;

                var record = parser.Read();

                Assert.NotNull(record);
                Assert.Equal(3, record.Length);
                Assert.Equal("1", record[0]);
                Assert.Equal("two \"2", record[1]);
                Assert.Equal("3", record[2]);

                Assert.Null(parser.Read());
            }
        }

        [Fact]
        public void ParseEscapedFieldWithEscapedQuoteTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                // 1,"two "" 2",3
                writer.WriteLine("1,\"two \"\" 2\",3");
                writer.Flush();
                stream.Position = 0;

                var record = parser.Read();

                Assert.NotNull(record);
                Assert.Equal(3, record.Length);
                Assert.Equal("1", record[0]);
                Assert.Equal("two \" 2", record[1]);
                Assert.Equal("3", record[2]);
            }
        }

        [Fact]
        public void ParserSepCrLfTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.Write("sep=;\r\n");
                writer.Write("1;2;3\r\n");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.HasExcelSeparator = true;
                var record = parser.Read();

                Assert.NotNull(record);
                Assert.Equal("1", record[0]);
                Assert.Equal("2", record[1]);
                Assert.Equal("3", record[2]);
            }
        }

        [Fact]
        public void ParserSepCrTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.Write("sep=;\r");
                writer.Write("1;2;3\r");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.HasExcelSeparator = true;
                var record = parser.Read();

                Assert.NotNull(record);
                Assert.Equal("1", record[0]);
                Assert.Equal("2", record[1]);
                Assert.Equal("3", record[2]);
            }
        }

        [Fact]
        public void ParserSepLfTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.Write("sep=;\n");
                writer.Write("1;2;3\n");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.HasExcelSeparator = true;
                var record = parser.Read();

                Assert.NotNull(record);
                Assert.Equal("1", record[0]);
                Assert.Equal("2", record[1]);
                Assert.Equal("3", record[2]);
            }
        }

        [Fact]
        public void WriteRecordsSepTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.Delimiter = ";";
                csv.Configuration.HasExcelSeparator = true;
                var list = new List<Simple>
                {
                    new Simple
                    {
                        Id = 1,
                        Name = "one"
                    }
                };
                csv.WriteRecords(list);
                writer.Flush();
                stream.Position = 0;

                var text = reader.ReadToEnd();

                Assert.Equal("sep=;\r\nId;Name\r\n1;one\r\n", text);
            }
        }

        [Fact]
        public void WriteRecordSepTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                var record = new Simple
                {
                    Id = 1,
                    Name = "one"
                };

                csv.Configuration.Delimiter = ";";
                csv.Configuration.HasExcelSeparator = true;
                csv.WriteExcelSeparator();
                csv.NextRecord();
                csv.WriteHeader<Simple>();
                csv.NextRecord();
                csv.WriteRecord(record);
                csv.NextRecord();

                writer.Flush();
                stream.Position = 0;

                var text = reader.ReadToEnd();

                Assert.Equal("sep=;\r\nId;Name\r\n1;one\r\n", text);
            }
        }

        [Fact]
        public void ParseFieldMissingQuoteGoesToEndOfFileTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.WriteLine("a,b,\"c");
                writer.WriteLine("d,e,f");
                writer.Flush();
                stream.Position = 0;

                var row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal("a", row[0]);
                Assert.Equal("b", row[1]);
                Assert.Equal("c\r\nd,e,f\r\n", row[2]);
            }
        }

        private class Simple
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}