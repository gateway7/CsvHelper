// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com


#if WINRT_4_5
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else

#endif

namespace CsvHelper.Tests.TypeConversion
{
    using System.Globalization;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;
    using Xunit;

    public class BooleanConverterTests
    {
        [Fact]
        public void ConvertToStringTest()
        {
            var converter = new BooleanConverter();

            var propertyMapData = new CsvPropertyMapData(null)
            {
                TypeConverter = converter,
                TypeConverterOptions = { CultureInfo = CultureInfo.CurrentCulture }
            };

            Assert.Equal("True", converter.ConvertToString(true, null, propertyMapData));

            Assert.Equal("False", converter.ConvertToString(false, null, propertyMapData));

            Assert.Equal("", converter.ConvertToString(null, null, propertyMapData));
            Assert.Equal("1", converter.ConvertToString(1, null, propertyMapData));
        }

        [Fact]
        public void ConvertFromStringTest()
        {
            var converter = new BooleanConverter();

            var propertyMapData = new CsvPropertyMapData(null) { TypeConverterOptions = { CultureInfo = CultureInfo.CurrentCulture } };

            Assert.True((bool)converter.ConvertFromString("true", null, propertyMapData));
            Assert.True((bool)converter.ConvertFromString("True", null, propertyMapData));
            Assert.True((bool)converter.ConvertFromString("TRUE", null, propertyMapData));
            Assert.True((bool)converter.ConvertFromString("1", null, propertyMapData));
            Assert.True((bool)converter.ConvertFromString("yes", null, propertyMapData));
            Assert.True((bool)converter.ConvertFromString("YES", null, propertyMapData));
            Assert.True((bool)converter.ConvertFromString("y", null, propertyMapData));
            Assert.True((bool)converter.ConvertFromString("Y", null, propertyMapData));
            Assert.True((bool)converter.ConvertFromString(" true ", null, propertyMapData));
            Assert.True((bool)converter.ConvertFromString(" yes ", null, propertyMapData));
            Assert.True((bool)converter.ConvertFromString(" y ", null, propertyMapData));

            Assert.False((bool)converter.ConvertFromString("false", null, propertyMapData));
            Assert.False((bool)converter.ConvertFromString("False", null, propertyMapData));
            Assert.False((bool)converter.ConvertFromString("FALSE", null, propertyMapData));
            Assert.False((bool)converter.ConvertFromString("0", null, propertyMapData));
            Assert.False((bool)converter.ConvertFromString("no", null, propertyMapData));
            Assert.False((bool)converter.ConvertFromString("NO", null, propertyMapData));
            Assert.False((bool)converter.ConvertFromString("n", null, propertyMapData));
            Assert.False((bool)converter.ConvertFromString("N", null, propertyMapData));
            Assert.False((bool)converter.ConvertFromString(" false ", null, propertyMapData));
            Assert.False((bool)converter.ConvertFromString(" 0 ", null, propertyMapData));
            Assert.False((bool)converter.ConvertFromString(" no ", null, propertyMapData));
            Assert.False((bool)converter.ConvertFromString(" n ", null, propertyMapData));

            Assert.Throws<CsvTypeConverterException>(() => converter.ConvertFromString(null, null, propertyMapData));
        }
    }
}