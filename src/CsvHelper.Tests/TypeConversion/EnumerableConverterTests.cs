// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.Tests.TypeConversion
{
    using System.Globalization;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;
    using Xunit;

    public class EnumerableConverterTests
    {
        [Fact]
        public void ConvertTest()
        {
            var converter = new EnumerableConverter();
            var propertyMapData = new CsvPropertyMapData(null) { TypeConverterOptions = { CultureInfo = CultureInfo.CurrentCulture } };

            Assert.Throws<CsvTypeConverterException>(() => converter.ConvertFromString("", null, propertyMapData));
            Assert.Throws<CsvTypeConverterException>(() => converter.ConvertToString(5, null, propertyMapData));
        }
    }
}