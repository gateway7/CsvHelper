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

    public class ByteConverterTests
    {
        [Fact]
        public void ConvertToStringTest()
        {
            var converter = new ByteConverter();
            var propertyMapData = new CsvPropertyMapData(null)
            {
                TypeConverter = converter,
                TypeConverterOptions = { CultureInfo = CultureInfo.CurrentCulture }
            };

            Assert.Equal("123", converter.ConvertToString((byte)123, null, propertyMapData));

            Assert.Equal("", converter.ConvertToString(null, null, propertyMapData));
        }

        [Fact]
        public void ConvertFromStringTest()
        {
            var converter = new ByteConverter();

            var propertyMapData = new CsvPropertyMapData(null);
            propertyMapData.TypeConverterOptions.CultureInfo = CultureInfo.CurrentCulture;

            Assert.Equal((byte)123, converter.ConvertFromString("123", null, propertyMapData));
            Assert.Equal((byte)123, converter.ConvertFromString(" 123 ", null, propertyMapData));

            Assert.Throws<CsvTypeConverterException>(() => converter.ConvertFromString(null, null, propertyMapData));
        }
    }
}