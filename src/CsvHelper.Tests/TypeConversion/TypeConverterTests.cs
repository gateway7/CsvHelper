﻿// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com


#if WINRT_4_5
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else

#endif

namespace CsvHelper.Tests.TypeConversion
{
    using System.Collections.Generic;
    using System.Linq;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;
    using Mocks;
    using Xunit;

    public class TypeConverterTests
    {
        [Fact]
        public void ReaderInheritedConverter()
        {
            var queue = new Queue<string[]>();
            queue.Enqueue(new[] { "1" });
            queue.Enqueue(null);
            var parserMock = new ParserMock(queue);
            var csv = new CsvReader(parserMock);
            csv.Configuration.HasHeaderRecord = false;
            csv.Configuration.RegisterClassMap<TestMap>();
            var list = csv.GetRecords<Test>().ToList();
        }

        private class Test
        {
            public int IntColumn { get; set; }
        }

        private sealed class TestMap : CsvClassMap<Test>
        {
            public TestMap()
            {
                Map(m => m.IntColumn).Index(0).TypeConverter<Converter>();
            }
        }

        private class Converter : Int32Converter {}
    }
}