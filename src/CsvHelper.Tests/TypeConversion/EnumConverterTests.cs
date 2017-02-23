// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.Tests.TypeConversion
{
    using System;
    using System.Globalization;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;
    using Xunit;

    public class EnumConverterTests
    {
        [Fact]
        public void ConstructorTest()
        {
            Assert.Throws<ArgumentException>(() => new EnumConverter(typeof(string)));
        }

        [Fact]
        public void ConvertToStringTest()
        {
            var converter = new EnumConverter(typeof(TestEnum));

            var propertyMapData =
                new CsvPropertyMapData(null)
                {
                    TypeConverter = converter,
                    TypeConverterOptions = { CultureInfo = CultureInfo.CurrentCulture }
                };

            Assert.Equal("None", converter.ConvertToString((TestEnum)0, null, propertyMapData));
            Assert.Equal("None", converter.ConvertToString(TestEnum.None, null, propertyMapData));
            Assert.Equal("One", converter.ConvertToString((TestEnum)1, null, propertyMapData));
            Assert.Equal("One", converter.ConvertToString(TestEnum.One, null, propertyMapData));
            Assert.Equal("", converter.ConvertToString(null, null, propertyMapData));
        }

        [Fact]
        public void ConvertFromStringTest()
        {
            var converter = new EnumConverter(typeof(TestEnum));

            var propertyMapData = new CsvPropertyMapData(null) { TypeConverterOptions = { CultureInfo = CultureInfo.CurrentCulture } };

            Assert.Equal(TestEnum.One, converter.ConvertFromString("One", null, propertyMapData));
            Assert.Equal(TestEnum.One, converter.ConvertFromString("one", null, propertyMapData));
            Assert.Equal(TestEnum.One, converter.ConvertFromString("1", null, propertyMapData));

            Assert.Throws<CsvTypeConverterException>(() => converter.ConvertFromString("", null, propertyMapData));
            Assert.Throws<CsvTypeConverterException>(() => converter.ConvertFromString(null, null, propertyMapData));
        }

        private enum TestEnum
        {
            None = 0,
            One = 1
        }
    }
}