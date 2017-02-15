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

    public class DateTimeConverterTests
    {
        [Fact]
        public void ConvertToStringTest()
        {
            var converter = new DateTimeConverter();
            var propertyMapData = new CsvPropertyMapData(null)
            {
                TypeConverter = converter,
                TypeConverterOptions = { CultureInfo = CultureInfo.CurrentCulture }
            };

            var dateTime = DateTime.Now;

            // Valid conversions.
            Assert.Equal(dateTime.ToString(), converter.ConvertToString(dateTime, null, propertyMapData));

            // Invalid conversions.
            Assert.Equal("1", converter.ConvertToString(1, null, propertyMapData));
            Assert.Equal("", converter.ConvertToString(null, null, propertyMapData));
        }

        [Fact]
        public void ConvertFromStringTest()
        {
            var converter = new DateTimeConverter();

            var propertyMapData = new CsvPropertyMapData(null) { TypeConverterOptions = { CultureInfo = CultureInfo.CurrentCulture } };

            var dateTime = DateTime.Now;

            // Valid conversions.
            Assert.Equal(dateTime.ToString(), converter.ConvertFromString(dateTime.ToString(), null, propertyMapData).ToString());
            Assert.Equal(dateTime.ToString(), converter.ConvertFromString(dateTime.ToString("o"), null, propertyMapData).ToString());
            Assert.Equal(dateTime.ToString(), converter.ConvertFromString(" " + dateTime + " ", null, propertyMapData).ToString());

            // Invalid conversions.
            Assert.Throws<CsvTypeConverterException>(() => converter.ConvertFromString(null, null, propertyMapData));
        }

        [Fact]
        public void ComponentModelCompatibilityTest()
        {
            var converter = new DateTimeConverter();
            var cmConverter = new System.ComponentModel.DateTimeConverter();

            var propertyMapData = new CsvPropertyMapData(null) { TypeConverterOptions = { CultureInfo = CultureInfo.CurrentCulture } };

            Assert.Throws<NotSupportedException>(() => cmConverter.ConvertFromString(null));
            Assert.Throws<CsvTypeConverterException>(() => converter.ConvertFromString(null, null, propertyMapData));
            Assert.Throws<FormatException>(() => cmConverter.ConvertFromString("blah"));
            Assert.Throws<FormatException>(() => converter.ConvertFromString("blah", null, propertyMapData));
        }
    }
}