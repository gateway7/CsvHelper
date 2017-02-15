// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.Tests
{
    using System;
    using System.IO;
    using CsvHelper.Configuration;
    using Xunit;

    public class CsvReaderConstructorTests
    {
        [Fact]
        public void InvalidParameterTest()
        {
            Assert.Throws<CsvConfigurationException>(() => new CsvReader(new TestParser(0)));
        }

        [Fact]
        public void EnsureInternalsAreSetupCorrectlyWhenPassingTextReaderTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                Assert.Same(csv.Configuration, csv.Parser.Configuration);
            }
        }

        [Fact]
        public void EnsureInternalsAreSetupCorrectlyWhenPassingTextReaderAndConfigurationTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, new CsvConfiguration()))
            {
                Assert.Same(csv.Configuration, csv.Parser.Configuration);
            }
        }

        [Fact]
        public void EnsureInternalsAreSetupCorrectlyWhenPassingParserTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            {
                var parser = new CsvParser(reader);

                using (var csv = new CsvReader(parser))
                {
                    Assert.Same(csv.Configuration, csv.Parser.Configuration);
                    Assert.Same(parser, csv.Parser);
                }
            }
        }

        private class TestParser : ICsvParser
        {
            public TestParser(long bytePosition)
            {
                BytePosition = bytePosition;
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public TextReader TextReader { get; }

            public ICsvParserConfiguration Configuration { get; private set; }

            public int FieldCount
            {
                get { throw new NotImplementedException(); }
            }

            public int RawRow { get; private set; }

            public string RawRecord { get; private set; }

            public string[] Read()
            {
                throw new NotImplementedException();
            }

            public long CharPosition
            {
                get { throw new NotImplementedException(); }
            }

            public long BytePosition { get; }

            public int Row
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}