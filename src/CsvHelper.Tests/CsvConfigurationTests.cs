// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com


#if WINRT_4_5
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else

#endif

namespace CsvHelper.Tests
{
    using System;
    using System.IO;
    using CsvHelper.Configuration;
    using Xunit;

    public class CsvConfigurationTests
    {
        [Fact]
        public void EnsureReaderAndParserConfigIsAreSameTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            {
                var csvReader = new CsvReader(reader);

                Assert.Same(csvReader.Configuration, csvReader.Parser.Configuration);

                var config = new CsvConfiguration();
                var parser = new CsvParser(reader, config);
                csvReader = new CsvReader(parser);

                Assert.Same(csvReader.Configuration, csvReader.Parser.Configuration);
            }
        }

        [Fact]
        public void AddingMappingsWithGenericMethod1Test()
        {
            var config = new CsvConfiguration();
            config.RegisterClassMap<TestClassMappings>();

            Assert.Equal(2, config.Maps[typeof(TestClass)].PropertyMaps.Count);
        }

        [Fact]
        public void AddingMappingsWithGenericMethod2Test()
        {
            var config = new CsvConfiguration();
            config.RegisterClassMap<TestClassMappings>();

            Assert.Equal(2, config.Maps[typeof(TestClass)].PropertyMaps.Count);
        }

        [Fact]
        public void AddingMappingsWithNonGenericMethodTest()
        {
            var config = new CsvConfiguration();
            config.RegisterClassMap(typeof(TestClassMappings));

            Assert.Equal(2, config.Maps[typeof(TestClass)].PropertyMaps.Count);
        }

        [Fact]
        public void AddingMappingsWithInstanceMethodTest()
        {
            var config = new CsvConfiguration();
            config.RegisterClassMap(new TestClassMappings());

            Assert.Equal(2, config.Maps[typeof(TestClass)].PropertyMaps.Count);
        }

        [Fact]
        public void RegisterClassMapGenericTest()
        {
            var config = new CsvConfiguration();

            Assert.Null(config.Maps[typeof(TestClass)]);
            config.RegisterClassMap<TestClassMappings>();
            Assert.NotNull(config.Maps[typeof(TestClass)]);
        }

        [Fact]
        public void RegisterClassMapNonGenericTest()
        {
            var config = new CsvConfiguration();

            Assert.Null(config.Maps[typeof(TestClass)]);
            config.RegisterClassMap(typeof(TestClassMappings));
            Assert.NotNull(config.Maps[typeof(TestClass)]);
        }

        [Fact]
        public void RegisterClassInstanceTest()
        {
            var config = new CsvConfiguration();

            Assert.Null(config.Maps[typeof(TestClass)]);
            config.RegisterClassMap(new TestClassMappings());
            Assert.NotNull(config.Maps[typeof(TestClass)]);
        }

        [Fact]
        public void UnregisterClassMapGenericTest()
        {
            var config = new CsvConfiguration();

            Assert.Null(config.Maps[typeof(TestClass)]);
            config.RegisterClassMap<TestClassMappings>();
            Assert.NotNull(config.Maps[typeof(TestClass)]);

            config.UnregisterClassMap<TestClassMappings>();
            Assert.Null(config.Maps[typeof(TestClass)]);
        }

        [Fact]
        public void UnregisterClassNonMapGenericTest()
        {
            var config = new CsvConfiguration();

            Assert.Null(config.Maps[typeof(TestClass)]);
            config.RegisterClassMap(typeof(TestClassMappings));
            Assert.NotNull(config.Maps[typeof(TestClass)]);

            config.UnregisterClassMap(typeof(TestClassMappings));
            Assert.Null(config.Maps[typeof(TestClass)]);
        }

        [Fact]
        public void AddingMappingsWithNonGenericMethodThrowsWhenNotACsvClassMap()
        {
            Assert.Throws<ArgumentException>(() => new CsvConfiguration().RegisterClassMap(typeof(TestClass)));
        }

        private class TestClass
        {
            public string StringColumn { get; set; }

            public int IntColumn { get; set; }
        }

        private class TestClassMappings : CsvClassMap<TestClass>
        {
            public TestClassMappings()
            {
                Map(c => c.StringColumn);
                Map(c => c.IntColumn);
            }
        }
    }
}