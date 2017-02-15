namespace CsvHelper.Tests
{
    using Xunit;

    public class RecordBuilderTests
    {
        [Fact]
        public void SetsDefaultCapacityInDefaultConstructorTest()
        {
            var rb = new RecordBuilder();
            Assert.Equal(16, rb.Capacity);
        }

        [Fact]
        public void SetsDefaultCapacityWhenZeroCapacityIsGivenInConstructorTest()
        {
            var rb = new RecordBuilder(0);
            Assert.Equal(16, rb.Capacity);
        }

        [Fact]
        public void SetsCapacityWhenGivenInConstructorTest()
        {
            var rb = new RecordBuilder(1);
            Assert.Equal(1, rb.Capacity);
        }

        [Fact]
        public void ResizeTest()
        {
            var rb = new RecordBuilder(2);

            rb.Add("1");
            Assert.Equal(1, rb.Length);
            Assert.Equal(2, rb.Capacity);

            rb.Add("2");
            Assert.Equal(2, rb.Length);
            Assert.Equal(2, rb.Capacity);

            rb.Add("3");
            Assert.Equal(3, rb.Length);
            Assert.Equal(4, rb.Capacity);
        }

        [Fact]
        public void ClearTest()
        {
            var rb = new RecordBuilder(1);
            rb.Add("1");
            rb.Add("2");
            rb.Clear();
            var array = rb.ToArray();

            Assert.Equal(0, rb.Length);
            Assert.Equal(2, rb.Capacity);
            Assert.Equal(0, array.Length);
        }

        [Fact]
        public void ToArrayTest()
        {
            var rb = new RecordBuilder();

            var array = rb.ToArray();
            Assert.Equal(0, array.Length);

            rb.Add("1");
            array = rb.ToArray();
            Assert.Equal(1, array.Length);
        }
    }
}