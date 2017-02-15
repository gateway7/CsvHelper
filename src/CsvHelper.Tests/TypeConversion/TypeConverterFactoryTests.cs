// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.Tests.TypeConversion
{
    using System;
    using CsvHelper.TypeConversion;
    using Xunit;

    public class TypeConverterFactoryTests
    {
        [Fact]
        public void GetConverterForUnknownTypeTest()
        {
            Assert.IsType<DefaultTypeConverter>(TypeConverterFactory.GetConverter(typeof(TestUnknownClass)));
        }

        [Fact]
        public void GetConverterForKnownTypeTest()
        {
            Assert.IsType<DefaultTypeConverter>(TypeConverterFactory.GetConverter<TestKnownClass>());

            TypeConverterFactory.AddConverter<TestKnownClass>(new TestKnownConverter());

            Assert.IsType<TestKnownConverter>(TypeConverterFactory.GetConverter<TestKnownClass>());
        }

        [Fact]
        public void RemoveConverterForUnknownTypeTest()
        {
            TypeConverterFactory.RemoveConverter<TestUnknownClass>();
            TypeConverterFactory.RemoveConverter(typeof(TestUnknownClass));
        }

        [Fact]
        public void GetConverterForByteTest()
        {
            Assert.IsType<ByteConverter>(TypeConverterFactory.GetConverter(typeof(byte)));
        }

        [Fact]
        public void GetConverterForCharTest()
        {
            Assert.IsType<CharConverter>(TypeConverterFactory.GetConverter(typeof(char)));
        }

        [Fact]
        public void GetConverterForDateTimeTest()
        {
            Assert.IsType<DateTimeConverter>(TypeConverterFactory.GetConverter(typeof(DateTime)));
        }

        [Fact]
        public void GetConverterForDecimalTest()
        {
            Assert.IsType<DecimalConverter>(TypeConverterFactory.GetConverter(typeof(decimal)));
        }

        [Fact]
        public void GetConverterForDoubleTest()
        {
            Assert.IsType<DoubleConverter>(TypeConverterFactory.GetConverter(typeof(double)));
        }

        [Fact]
        public void GetConverterForFloatTest()
        {
            Assert.IsType<SingleConverter>(TypeConverterFactory.GetConverter(typeof(float)));
        }

        [Fact]
        public void GetConverterForGuidTest()
        {
            Assert.IsType<GuidConverter>(TypeConverterFactory.GetConverter(typeof(Guid)));
        }

        [Fact]
        public void GetConverterForInt16Test()
        {
            Assert.IsType<Int16Converter>(TypeConverterFactory.GetConverter(typeof(short)));
        }

        [Fact]
        public void GetConverterForInt32Test()
        {
            Assert.IsType<Int32Converter>(TypeConverterFactory.GetConverter(typeof(int)));
        }

        [Fact]
        public void GetConverterForInt64Test()
        {
            Assert.IsType<Int64Converter>(TypeConverterFactory.GetConverter(typeof(long)));
        }

        [Fact]
        public void GetConverterForNullableTest()
        {
            Assert.IsType<NullableConverter>(TypeConverterFactory.GetConverter(typeof(int?)));
        }

        [Fact]
        public void GetConverterForSByteTest()
        {
            Assert.IsType<SByteConverter>(TypeConverterFactory.GetConverter(typeof(sbyte)));
        }

        [Fact]
        public void GetConverterForStringTest()
        {
            Assert.IsType<StringConverter>(TypeConverterFactory.GetConverter(typeof(string)));
        }

        [Fact]
        public void GetConverterForUInt16Test()
        {
            Assert.IsType<UInt16Converter>(TypeConverterFactory.GetConverter(typeof(ushort)));
        }

        [Fact]
        public void GetConverterForUInt32Test()
        {
            Assert.IsType<UInt32Converter>(TypeConverterFactory.GetConverter(typeof(uint)));
        }

        [Fact]
        public void GetConverterForUInt64Test()
        {
            Assert.IsType<UInt64Converter>(TypeConverterFactory.GetConverter(typeof(ulong)));
        }

        [Fact]
        public void GetConverterForEnumTest()
        {
            Assert.IsType<EnumConverter>(TypeConverterFactory.GetConverter(typeof(TestEnum)));
        }

        private class TestListConverter : DefaultTypeConverter {}

        private class TestUnknownClass {}

        private class TestKnownClass {}

        private class TestKnownConverter : DefaultTypeConverter {}

        private enum TestEnum
        {}
    }
}