﻿// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com


#if WINRT_4_5
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else

#endif

namespace CsvHelper.Tests
{
    using CsvHelper.Configuration;
    using Xunit;

    public class CsvClassMappingAutoMapTests
    {
        [Fact]
        public void Test()
        {
            var aMap = new AMap();

            Assert.Equal(3, aMap.PropertyMaps.Count);
            Assert.Equal(0, aMap.PropertyMaps[0].Data.Index);
            Assert.Equal(1, aMap.PropertyMaps[1].Data.Index);
            Assert.Equal(2, aMap.PropertyMaps[2].Data.Index);
            Assert.True(aMap.PropertyMaps[2].Data.Ignore);

            Assert.Equal(1, aMap.ReferenceMaps.Count);
        }

        private class A
        {
            public int One { get; set; }

            public int Two { get; set; }

            public int Three { get; set; }

            public B B { get; set; }
        }

        private class B
        {
            public int Four { get; set; }

            public int Five { get; set; }

            public int Six { get; set; }
        }

        private sealed class AMap : CsvClassMap<A>
        {
            public AMap()
            {
                AutoMap();
                Map(m => m.Three).Ignore();
            }
        }

        private sealed class BMap : CsvClassMap<B> {}
    }
}