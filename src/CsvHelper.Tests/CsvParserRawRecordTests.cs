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
    using Xunit;

    public class CsvParserRawRecordTests
    {
        [Fact]
        public void RawRecordCrLfTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.Write("1,2\r\n");
                writer.Write("3,4\r\n");
                writer.Flush();
                stream.Position = 0;

                parser.Read();
                Assert.Equal("1,2\r\n", parser.RawRecord);

                parser.Read();
                Assert.Equal("3,4\r\n", parser.RawRecord);

                parser.Read();
                Assert.Equal(string.Empty, parser.RawRecord);
            }
        }

        [Fact]
        public void RawRecordCrTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.Write("1,2\r");
                writer.Write("3,4\r");
                writer.Flush();
                stream.Position = 0;

                parser.Read();
                Assert.Equal("1,2\r", parser.RawRecord);

                parser.Read();
                Assert.Equal("3,4\r", parser.RawRecord);

                parser.Read();
                Assert.Equal(string.Empty, parser.RawRecord);
            }
        }

        [Fact]
        public void RawRecordLfTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.Write("1,2\n");
                writer.Write("3,4\n");
                writer.Flush();
                stream.Position = 0;

                parser.Read();
                Assert.Equal("1,2\n", parser.RawRecord);

                parser.Read();
                Assert.Equal("3,4\n", parser.RawRecord);

                parser.Read();
                Assert.Equal(string.Empty, parser.RawRecord);
            }
        }

        [Fact]
        public void RawRecordCr2DelimiterTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.Write("1;;2\r");
                writer.Write("3;;4\r");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.Delimiter = ";;";

                parser.Read();
                Assert.Equal("1;;2\r", parser.RawRecord);

                parser.Read();
                Assert.Equal("3;;4\r", parser.RawRecord);

                parser.Read();
                Assert.Equal(string.Empty, parser.RawRecord);
            }
        }

        [Fact]
        public void TinyBufferTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var parser = new CsvParser(reader))
            {
                writer.Write("1,2\r\n");
                writer.Write("3,4\r\n");
                writer.Flush();
                stream.Position = 0;

                parser.Configuration.BufferSize = 1;

                parser.Read();
                Assert.Equal("1,2\r\n", parser.RawRecord);

                parser.Read();
                Assert.Equal("3,4\r\n", parser.RawRecord);

                parser.Read();
                Assert.Equal(string.Empty, parser.RawRecord);
            }
        }
    }
}