namespace CsvHelper.Tests.Reading
{
    using System;
    using System.Collections.Generic;
    using Mocks;
    using Xunit;

    public class TryGetTests
    {
        [Fact]
        public void TryGetFieldInvalidIndexTest()
        {
            var data1 = new[] { "One", "Two" };
            var data2 = new[] { "one", "two" };
            var queue = new Queue<string[]>();
            queue.Enqueue(data1);
            queue.Enqueue(data2);
            queue.Enqueue(null);
            var parserMock = new ParserMock(queue);

            var reader = new CsvReader(parserMock);
            reader.Read();

            var got = reader.TryGetField(0, out int field);
            Assert.False(got);
            Assert.Equal(default(int), field);
        }

        [Fact]
        public void TryGetFieldInvalidNameTest()
        {
            var data1 = new[] { "One", "Two" };
            var data2 = new[] { "one", "two" };
            var queue = new Queue<string[]>();
            queue.Enqueue(data1);
            queue.Enqueue(data2);
            queue.Enqueue(null);
            var parserMock = new ParserMock(queue);

            var reader = new CsvReader(parserMock);
            reader.Read();

            var got = reader.TryGetField("One", out int field);
            Assert.False(got);
            Assert.Equal(default(int), field);
        }

        [Fact]
        public void TryGetFieldTest()
        {
            var data1 = new[] { "One", "Two" };
            var data2 = new[] { "1", "2" };
            var queue = new Queue<string[]>();
            queue.Enqueue(data1);
            queue.Enqueue(data2);
            queue.Enqueue(null);
            var parserMock = new ParserMock(queue);

            var reader = new CsvReader(parserMock);
            reader.Read();
            reader.ReadHeader();
            reader.Read();

            var got = reader.TryGetField(0, out int field);
            Assert.True(got);
            Assert.Equal(1, field);
        }

        [Fact]
        public void TryGetFieldStrictTest()
        {
            var data1 = new[] { "One", "Two" };
            var data2 = new[] { "1", "2" };
            var queue = new Queue<string[]>();
            queue.Enqueue(data1);
            queue.Enqueue(data2);
            queue.Enqueue(null);
            var parserMock = new ParserMock(queue);

            var reader = new CsvReader(parserMock) { Configuration = { WillThrowOnMissingField = true } };
            reader.Read();
            reader.ReadHeader();
            reader.Read();

            var got = reader.TryGetField("One", out int field);
            Assert.True(got);
            Assert.Equal(1, field);
        }

        [Fact]
        public void TryGetFieldEmptyDate()
        {
            // DateTimeConverter.IsValid() doesn't work correctly
            // so we need to test and make sure that the conversion
            // fails for an empty string for a date.
            var data = new[] { " " };
            var queue = new Queue<string[]>();
            queue.Enqueue(data);
            queue.Enqueue(null);
            var parserMock = new ParserMock(queue);

            var reader = new CsvReader(parserMock);
            reader.Configuration.HasHeaderRecord = false;
            reader.Read();

            var got = reader.TryGetField(0, out DateTime field);

            Assert.False(got);
            Assert.Equal(DateTime.MinValue, field);
        }

        [Fact]
        public void TryGetNullableFieldEmptyDate()
        {
            // DateTimeConverter.IsValid() doesn't work correctly
            // so we need to test and make sure that the conversion
            // fails for an emptry string for a date.
            var data = new[] { " " };
            var queue = new Queue<string[]>();
            queue.Enqueue(data);
            queue.Enqueue(null);
            var parserMock = new ParserMock(queue);

            var reader = new CsvReader(parserMock);
            reader.Configuration.HasHeaderRecord = false;
            reader.Read();

            var got = reader.TryGetField(0, out DateTime? field);

            Assert.False(got);
            Assert.Null(field);
        }

        [Fact]
        public void TryGetDoesNotThrowWhenWillThrowOnMissingFieldIsEnabled()
        {
            var data = new[] { "1" };
            var queue = new Queue<string[]>();
            queue.Enqueue(data);
            queue.Enqueue(null);
            var parserMock = new ParserMock(queue);

            var reader = new CsvReader(parserMock);
            reader.Configuration.WillThrowOnMissingField = true;
            reader.Read();
            Assert.False(reader.TryGetField("test", out string field));
        }

        [Fact]
        public void TryGetFieldIndexTest()
        {
            var parserMock = new ParserMock
            {
                { "One", "Two", "Two" },
                { "1", "2", "3" }
            };
            var reader = new CsvReader(parserMock);
            reader.Read();
            reader.ReadHeader();
            reader.Read();

            var got = reader.TryGetField("Two", 0, out int field);
            Assert.True(got);
            Assert.Equal(2, field);

            got = reader.TryGetField("Two", 1, out field);
            Assert.True(got);
            Assert.Equal(3, field);
        }
    }
}