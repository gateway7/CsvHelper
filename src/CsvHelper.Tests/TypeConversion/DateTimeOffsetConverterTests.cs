namespace CsvHelper.Tests.TypeConversion
{
    using System;
    using System.Globalization;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;
    using Xunit;

    public class DateTimeOffsetConverterTests
    {
        [Fact]
        public void ConvertToStringTest()
        {
            var converter = new DateTimeOffsetConverter();
            var propertyMapData = new CsvPropertyMapData(null)
            {
                TypeConverter = converter,
                TypeConverterOptions = { CultureInfo = CultureInfo.CurrentCulture }
            };

            var dateTime = DateTimeOffset.Now;

            // Valid conversions.
            Assert.Equal(dateTime.ToString(), converter.ConvertToString(dateTime, null, propertyMapData));

            // Invalid conversions.
            Assert.Equal("1", converter.ConvertToString(1, null, propertyMapData));
            Assert.Equal("", converter.ConvertToString(null, null, propertyMapData));
        }

        [Fact]
        public void ConvertFromStringTest()
        {
            var converter = new DateTimeOffsetConverter();

            var propertyMapData = new CsvPropertyMapData(null);
            propertyMapData.TypeConverterOptions.CultureInfo = CultureInfo.CurrentCulture;

            var dateTime = DateTimeOffset.Now;

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
            var converter = new DateTimeOffsetConverter();
            var cmConverter = new System.ComponentModel.DateTimeOffsetConverter();

            var propertyMapData = new CsvPropertyMapData(null) { TypeConverterOptions = { CultureInfo = CultureInfo.CurrentCulture } };

            Assert.Throws<NotSupportedException>(() => cmConverter.ConvertFromString(null));
            Assert.Throws<CsvTypeConverterException>(() => converter.ConvertFromString(null, null, propertyMapData));
            Assert.Throws<FormatException>(() => cmConverter.ConvertFromString("blah"));
            Assert.Throws<FormatException>(() => converter.ConvertFromString("blah", null, propertyMapData));
        }
    }
}