// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.Tests.Mappings
{
    using System;
    using System.IO;
    using System.Linq;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;
    using Xunit;

    public class CsvClassMappingTests
    {
        [Fact]
        public void MapTest()
        {
            var map = new TestMappingDefaultClass();

            //map.CreateMap();

            Assert.Equal(3, map.PropertyMaps.Count);

            Assert.Equal("GuidColumn", map.PropertyMaps[0].Data.Names.FirstOrDefault());
            Assert.Equal(0, map.PropertyMaps[0].Data.Index);
            Assert.Equal(typeof(GuidConverter), map.PropertyMaps[0].Data.TypeConverter.GetType());

            Assert.Equal("IntColumn", map.PropertyMaps[1].Data.Names.FirstOrDefault());
            Assert.Equal(1, map.PropertyMaps[1].Data.Index);
            Assert.Equal(typeof(Int32Converter), map.PropertyMaps[1].Data.TypeConverter.GetType());

            Assert.Equal("StringColumn", map.PropertyMaps[2].Data.Names.FirstOrDefault());
            Assert.Equal(2, map.PropertyMaps[2].Data.Index);
            Assert.Equal(typeof(StringConverter), map.PropertyMaps[2].Data.TypeConverter.GetType());
        }

        [Fact]
        public void MapAnnotatedTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("id,item_id,name,title");
                writer.WriteLine(@"1,101,testtest,""new title""");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.RegisterClass<AnnotatedClass>();

                var records = csv.GetRecords<AnnotatedClass>().ToList();

                Assert.True(records.Any());
                Assert.Equal(101, records[0].Id);
                Assert.Equal("new title", records[0].Name);
            }
        }

        [Fact]
        public void MapNameTest()
        {
            var map = new TestMappingNameClass();

            //map.CreateMap();

            Assert.Equal(3, map.PropertyMaps.Count);

            Assert.Equal("Guid Column", map.PropertyMaps[0].Data.Names.FirstOrDefault());
            Assert.Equal("Int Column", map.PropertyMaps[1].Data.Names.FirstOrDefault());
            Assert.Equal("String Column", map.PropertyMaps[2].Data.Names.FirstOrDefault());
        }

        [Fact]
        public void MapIndexTest()
        {
            var map = new TestMappingIndexClass();

            //map.CreateMap();

            Assert.Equal(3, map.PropertyMaps.Count);

            Assert.Equal(2, map.PropertyMaps[0].Data.Index);
            Assert.Equal(3, map.PropertyMaps[1].Data.Index);
            Assert.Equal(1, map.PropertyMaps[2].Data.Index);
        }

        [Fact]
        public void MapIgnoreTest()
        {
            var map = new TestMappingIngoreClass();

            //map.CreateMap();

            Assert.Equal(3, map.PropertyMaps.Count);

            Assert.True(map.PropertyMaps[0].Data.Ignore);
            Assert.False(map.PropertyMaps[1].Data.Ignore);
            Assert.True(map.PropertyMaps[2].Data.Ignore);
        }

        [Fact]
        public void MapTypeConverterTest()
        {
            var map = new TestMappingTypeConverterClass();

            //map.CreateMap();

            Assert.Equal(3, map.PropertyMaps.Count);

            Assert.IsType<Int16Converter>(map.PropertyMaps[0].Data.TypeConverter);
            Assert.IsType<StringConverter>(map.PropertyMaps[1].Data.TypeConverter);
            Assert.IsType<Int64Converter>(map.PropertyMaps[2].Data.TypeConverter);
        }

        [Fact]
        public void MapMultipleNamesTest()
        {
            var map = new TestMappingMultipleNamesClass();

            //map.CreateMap();

            Assert.Equal(3, map.PropertyMaps.Count);

            Assert.Equal(3, map.PropertyMaps[0].Data.Names.Count);
            Assert.Equal(3, map.PropertyMaps[1].Data.Names.Count);
            Assert.Equal(3, map.PropertyMaps[2].Data.Names.Count);

            Assert.Equal("guid1", map.PropertyMaps[0].Data.Names[0]);
            Assert.Equal("guid2", map.PropertyMaps[0].Data.Names[1]);
            Assert.Equal("guid3", map.PropertyMaps[0].Data.Names[2]);

            Assert.Equal("int1", map.PropertyMaps[1].Data.Names[0]);
            Assert.Equal("int2", map.PropertyMaps[1].Data.Names[1]);
            Assert.Equal("int3", map.PropertyMaps[1].Data.Names[2]);

            Assert.Equal("string1", map.PropertyMaps[2].Data.Names[0]);
            Assert.Equal("string2", map.PropertyMaps[2].Data.Names[1]);
            Assert.Equal("string3", map.PropertyMaps[2].Data.Names[2]);
        }

        [Fact]
        public void MapConstructorTest()
        {
            var map = new TestMappingConstructorClass();

            //map.CreateMap();

            Assert.NotNull(map.Constructor);
        }

        [Fact]
        public void MapMultipleTypesTest()
        {
            var config = new CsvConfiguration();
            config.RegisterClassMap<AMap>();
            config.RegisterClassMap<BMap>();

            Assert.NotNull(config.Maps[typeof(A)]);
            Assert.NotNull(config.Maps[typeof(B)]);
        }

        [Fact]
        public void PropertyMapAccessTest()
        {
            var config = new CsvConfiguration();
            config.RegisterClassMap<AMap>();
            config.Maps.Find<A>().Map(m => m.AId).Ignore();

            Assert.Equal(true, config.Maps[typeof(A)].PropertyMaps[0].Data.Ignore);
        }

        private class A
        {
            public int AId { get; set; }
        }

        private sealed class AMap : CsvClassMap<A>
        {
            public AMap()
            {
                Map(m => m.AId);
            }
        }

        private class B
        {
            public int BId { get; set; }
        }

        private sealed class BMap : CsvClassMap<B>
        {
            public BMap()
            {
                Map(m => m.BId);
            }
        }

        private class AnnotatedClass
        {
            [CsvField("item_id")]
            public int Id { get; set; }

            [CsvField("title")]
            public string Name { get; set; }

            public string Desription { get; set; }
        }

        private class TestClass
        {
            public string StringColumn { get; }

            public int IntColumn { get; set; }

            public Guid GuidColumn { get; set; }

            public string NotUsedColumn { get; set; }

            public TestClass() {}

            public TestClass(string stringColumn)
            {
                StringColumn = stringColumn;
            }
        }

        private sealed class TestMappingConstructorClass : CsvClassMap<TestClass>
        {
            public TestMappingConstructorClass()
            {
                ConstructUsing(() => new TestClass("String Column"));
            }
        }

        private sealed class TestMappingDefaultClass : CsvClassMap<TestClass>
        {
            public TestMappingDefaultClass()
            {
                Map(m => m.GuidColumn);
                Map(m => m.IntColumn);
                Map(m => m.StringColumn);
            }
        }

        private sealed class TestMappingNameClass : CsvClassMap<TestClass>
        {
            public TestMappingNameClass()
            {
                Map(m => m.GuidColumn).Name("Guid Column");
                Map(m => m.IntColumn).Name("Int Column");
                Map(m => m.StringColumn).Name("String Column");
            }
        }

        private sealed class TestMappingIndexClass : CsvClassMap<TestClass>
        {
            public TestMappingIndexClass()
            {
                Map(m => m.GuidColumn).Index(3);
                Map(m => m.IntColumn).Index(2);
                Map(m => m.StringColumn).Index(1);
            }
        }

        private sealed class TestMappingIngoreClass : CsvClassMap<TestClass>
        {
            public TestMappingIngoreClass()
            {
                Map(m => m.GuidColumn).Ignore();
                Map(m => m.IntColumn);
                Map(m => m.StringColumn).Ignore();
            }
        }

        private sealed class TestMappingTypeConverterClass : CsvClassMap<TestClass>
        {
            public TestMappingTypeConverterClass()
            {
                Map(m => m.GuidColumn).TypeConverter<Int16Converter>();
                Map(m => m.IntColumn).TypeConverter<StringConverter>();
                Map(m => m.StringColumn).TypeConverter(new Int64Converter());
            }
        }

        private sealed class TestMappingMultipleNamesClass : CsvClassMap<TestClass>
        {
            public TestMappingMultipleNamesClass()
            {
                Map(m => m.GuidColumn).Name("guid1", "guid2", "guid3");
                Map(m => m.IntColumn).Name("int1", "int2", "int3");
                Map(m => m.StringColumn).Name("string1", "string2", "string3");
            }
        }
    }
}