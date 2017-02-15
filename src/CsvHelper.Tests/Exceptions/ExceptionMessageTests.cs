namespace CsvHelper.Tests.Exceptions
{
    using System.Collections.Generic;
    using System.Linq;
    using CsvHelper.TypeConversion;
    using Mocks;
    using Xunit;

    public class ExceptionMessageTests
    {
        [Fact]
        public void GetMissingFieldTest()
        {
            var parser = new ParserMock
            {
                { "Id", "Name" },
                { "a", "b" },
                null
            };

            var reader = new CsvReader(parser);
            reader.Read();
            reader.Read();
            try
            {
                reader.GetField(2);
                Assert.True(false);
            }
            catch (CsvMissingFieldException ex)
            {
                Assert.Equal(2, ex.Row);
                Assert.Equal(2, ex.FieldIndex);
            }
        }

        [Fact]
        public void GetGenericMissingFieldWithTypeTest()
        {
            var parser = new ParserMock
            {
                { "Id", "Name" },
                { "a", "b" },
                null
            };

            var reader = new CsvReader(parser);
            reader.Read();
            reader.Read();
            try
            {
                reader.GetField<int>(2);
                Assert.True(false);
            }
            catch (CsvMissingFieldException ex)
            {
                Assert.Equal(2, ex.Row);
                Assert.Equal(ex.Type, typeof(int));
                Assert.Equal(2, ex.FieldIndex);
            }
        }

        [Fact]
        public void GetRecordGenericTest()
        {
            var parser = new ParserMock
            {
                { "Id", "Name" },
                { "a", "b" },
                null
            };

            var reader = new CsvReader(parser);
            reader.Read();
            try
            {
                reader.GetRecord<Simple>();
                Assert.True(false);
            }
            catch (CsvTypeConverterException ex)
            {
                //var expected = "Row: '2' (1 based)\r\n" +
                //        "Type: 'CsvHelper.Tests.Exceptions.ExceptionMessageTests+Simple'\r\n" +
                //        "Field Index: '0' (0 based)\r\n" +
                //        "Field Name: 'Id'\r\n" +
                //        "Field Value: 'a'\r\n";
                //Assert.Equal( expected, ex.Data["CsvHelper"] );

                Assert.Equal(2, ex.Row);
                Assert.Equal(typeof(Simple), ex.Type);
                Assert.Equal(0, ex.FieldIndex);
            }
        }

        [Fact]
        public void GetRecordTest()
        {
            var parser = new ParserMock
            {
                { "Id", "Name" },
                { "a", "b" },
                null
            };

            var reader = new CsvReader(parser);
            reader.Read();
            try
            {
                reader.GetRecord(typeof(Simple));
                Assert.True(false);
            }
            catch (CsvTypeConverterException ex)
            {
                Assert.Equal(2, ex.Row);
                Assert.Equal(typeof(Simple), ex.Type);
                Assert.Equal(0, ex.FieldIndex);
            }
        }

        [Fact]
        public void GetRecordsGenericTest()
        {
            var parser = new ParserMock
            {
                { "Id", "Name" },
                { "a", "b" },
                null
            };

            var reader = new CsvReader(parser);
            try
            {
                reader.GetRecords<Simple>().ToList();
                Assert.True(false);
            }
            catch (CsvTypeConverterException ex)
            {
                Assert.Equal(2, ex.Row);
                Assert.Equal(typeof(Simple), ex.Type);
                Assert.Equal(0, ex.FieldIndex);
            }
        }

        [Fact]
        public void GetRecordsTest()
        {
            var parser = new ParserMock
            {
                { "Id", "Name" },
                { "a", "b" },
                null
            };

            var reader = new CsvReader(parser);
            try
            {
                reader.GetRecords(typeof(Simple)).ToList();
                Assert.True(false);
            }
            catch (CsvTypeConverterException ex)
            {
                Assert.Equal(2, ex.Row);
                Assert.Equal(typeof(Simple), ex.Type);
                Assert.Equal(0, ex.FieldIndex);
            }
        }

        [Fact]
        public void GetFieldIndexTest()
        {
            var parser = new ParserMock
            {
                { "Id", "Name" },
                { "a", "b" },
                null
            };

            var reader = new CsvReader(parser);
            reader.Read();
            reader.ReadHeader();
            reader.Read();
            try
            {
                reader.GetField("c");
                Assert.True(false);
            }
            catch (CsvMissingFieldException ex)
            {
                Assert.Equal(2, ex.Row);
                Assert.Equal(-1, ex.FieldIndex);
            }
        }

        [Fact]
        public void WriteRecordGenericTest()
        {
            var serializer = new SerializerMock(true);
            var writer = new CsvWriter(serializer);

            try
            {
                writer.WriteRecord(new Simple());
                writer.NextRecord();
                Assert.True(false);
            }
            catch (CsvHelperException ex)
            {
                Assert.Equal(1, ex.Row);
            }
        }

        [Fact]
        public void WriteRecordsGenericTest()
        {
            var serializer = new SerializerMock(true);
            var writer = new CsvWriter(serializer);

            try
            {
                writer.WriteRecords(new List<Simple> { new Simple() });
                Assert.True(false);
            }
            catch (CsvHelperException ex)
            {
                Assert.Equal(typeof(Simple), ex.Type);
            }
        }

        private class Simple
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}