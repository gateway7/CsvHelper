namespace CsvHelper.Tests.Reflection
{
    using Xunit;

    public class GetPropertiesTests
    {
        [Fact]
        public void FirstLevelTest()
        {
            var stack = ReflectionHelper.GetMembers<A>(a => a.P1);

            Assert.Equal(1, stack.Count);
            Assert.Equal("P1", stack.Pop().Name);
        }

        [Fact]
        public void LastLevelTest()
        {
            var stack = ReflectionHelper.GetMembers<A>(a => a.B.C.D.P4);

            Assert.Equal(4, stack.Count);
            Assert.Equal("B", stack.Pop().Name);
            Assert.Equal("C", stack.Pop().Name);
            Assert.Equal("D", stack.Pop().Name);
            Assert.Equal("P4", stack.Pop().Name);
        }

        public void ThirdLevelTest() {}

        private class A
        {
            public string P1 { get; set; }

            public B B { get; set; }
        }

        private class B
        {
            public string P2 { get; set; }

            public C C { get; set; }
        }

        private class C
        {
            public string P3 { get; set; }

            public D D { get; set; }
        }

        private class D
        {
            public string P4 { get; set; }
        }
    }
}