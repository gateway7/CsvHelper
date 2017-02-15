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
    using CsvHelper.Configuration;
    using Xunit;

    public class CsvParserDelimiterTests
    {
        [Fact]
        public void DifferentDelimiterTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.WriteLine("1\t2\t3");
                writer.WriteLine("4\t5\t6");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.Delimiter = "\t";

                var row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(3, row.Length);
                Assert.Equal("1", row[0]);
                Assert.Equal("2", row[1]);
                Assert.Equal("3", row[2]);

                row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(3, row.Length);
                Assert.Equal("4", row[0]);
                Assert.Equal("5", row[1]);
                Assert.Equal("6", row[2]);

                row = parser.Read();
                Assert.Null(row);
            }
        }

        [Fact]
        public void MultipleCharDelimiter2Test()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.WriteLine("1``2``3");
                writer.WriteLine("4``5``6");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.Delimiter = "``";

                var row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(3, row.Length);
                Assert.Equal("1", row[0]);
                Assert.Equal("2", row[1]);
                Assert.Equal("3", row[2]);

                row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(3, row.Length);
                Assert.Equal("4", row[0]);
                Assert.Equal("5", row[1]);
                Assert.Equal("6", row[2]);

                row = parser.Read();
                Assert.Null(row);
            }
        }

        [Fact]
        public void MultipleCharDelimiter3Test()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.WriteLine("1`\t`2`\t`3");
                writer.WriteLine("4`\t`5`\t`6");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.Delimiter = "`\t`";

                var row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(3, row.Length);
                Assert.Equal("1", row[0]);
                Assert.Equal("2", row[1]);
                Assert.Equal("3", row[2]);

                row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(3, row.Length);
                Assert.Equal("4", row[0]);
                Assert.Equal("5", row[1]);
                Assert.Equal("6", row[2]);

                row = parser.Read();
                Assert.Null(row);
            }
        }

        [Fact]
        public void AllFieldsEmptyTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.WriteLine(";;;;");
                writer.WriteLine(";;;;");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.Delimiter = ";;";

                var row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(3, row.Length);
                Assert.Equal("", row[0]);
                Assert.Equal("", row[1]);
                Assert.Equal("", row[2]);

                row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(3, row.Length);
                Assert.Equal("", row[0]);
                Assert.Equal("", row[1]);
                Assert.Equal("", row[2]);

                row = parser.Read();
                Assert.Null(row);
            }
        }

        [Fact]
        public void AllFieldsEmptyNoEolOnLastLineTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.WriteLine(";;;;");
                writer.Write(";;;;");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.Delimiter = ";;";

                var row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(3, row.Length);
                Assert.Equal("", row[0]);
                Assert.Equal("", row[1]);
                Assert.Equal("", row[2]);

                row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(3, row.Length);
                Assert.Equal("", row[0]);
                Assert.Equal("", row[1]);
                Assert.Equal("", row[2]);

                row = parser.Read();
                Assert.Null(row);
            }
        }

        [Fact]
        public void EmptyLastFieldTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.WriteLine("1;;2;;");
                writer.WriteLine("4;;5;;");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.Delimiter = ";;";

                var row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(3, row.Length);
                Assert.Equal("1", row[0]);
                Assert.Equal("2", row[1]);
                Assert.Equal("", row[2]);

                row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(3, row.Length);
                Assert.Equal("4", row[0]);
                Assert.Equal("5", row[1]);
                Assert.Equal("", row[2]);

                row = parser.Read();
                Assert.Null(row);
            }
        }

        [Fact]
        public void EmptyLastFieldNoEolOnLastLineTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.WriteLine("1;;2;;");
                writer.Write("4;;5;;");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.Delimiter = ";;";

                var row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(3, row.Length);
                Assert.Equal("1", row[0]);
                Assert.Equal("2", row[1]);
                Assert.Equal("", row[2]);

                row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(3, row.Length);
                Assert.Equal("4", row[0]);
                Assert.Equal("5", row[1]);
                Assert.Equal("", row[2]);

                row = parser.Read();
                Assert.Null(row);
            }
        }

        [Fact]
        public void DifferentDelimiter2ByteCountTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.Write("1;;2\r\n");
                writer.Write("4;;5\r\n");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.Delimiter = ";;";
                parser.Configuration.CountBytes = true;

                parser.Read();
                Assert.Equal(6, parser.BytePosition);

                parser.Read();
                Assert.Equal(12, parser.BytePosition);

                Assert.Null(parser.Read());
            }
        }

        [Fact]
        public void DifferentDelimiter3ByteCountTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.Write("1;;;2\r\n");
                writer.Write("4;;;5\r\n");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.Delimiter = ";;;";
                parser.Configuration.CountBytes = true;

                parser.Read();
                Assert.Equal(7, parser.BytePosition);

                parser.Read();
                Assert.Equal(14, parser.BytePosition);

                Assert.Null(parser.Read());
            }
        }

        [Fact]
        public void MultipleCharDelimiterWithBufferEndingInMiddleOfDelimiterTest()
        {
            var config = new CsvConfiguration
            {
                Delimiter = "|~|",
                BufferSize = 3
            };

            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var parser = new CsvParser(reader, config))
            {
                writer.WriteLine("1|~|2");
                writer.Flush();
                stream.Position = 0;

                var row = parser.Read();
                Assert.NotNull(row);
                Assert.Equal(2, row.Length);
                Assert.Equal("1", row[0]);
                Assert.Equal("2", row[1]);
                row = parser.Read();
                Assert.Null(row);
            }
        }
    }
}