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

    public class CharConverterTests
    {
        [Fact]
        public void ConvertToStringTest()
        {
            var converter = new CharConverter();
            var propertyMapData = new CsvPropertyMapData(null)
            {
                TypeConverter = converter,
                TypeConverterOptions = { CultureInfo = CultureInfo.CurrentCulture }
            };

            Assert.Equal("a", converter.ConvertToString('a', null, propertyMapData));

            Assert.Equal("True", converter.ConvertToString(true, null, propertyMapData));

            Assert.Equal("", converter.ConvertToString(null, null, propertyMapData));
        }

        [Fact]
        public void ConvertFromStringTest()
        {
            var converter = new CharConverter();

            var propertyMapData = new CsvPropertyMapData(null) { TypeConverterOptions = { CultureInfo = CultureInfo.CurrentCulture } };

            Assert.Equal('a', converter.ConvertFromString("a", null, propertyMapData));
            Assert.Equal('a', converter.ConvertFromString(" a ", null, propertyMapData));
            Assert.Equal(' ', converter.ConvertFromString(" ", null, propertyMapData));

            Assert.Throws<CsvTypeConverterException>(() => converter.ConvertFromString(null, null, propertyMapData));
        }
    }
}