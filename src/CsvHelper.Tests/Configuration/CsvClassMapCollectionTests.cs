namespace CsvHelper.Tests.Configuration
{
    using CsvHelper.Configuration;
    using Xunit;

    public class CsvClassMapCollectionTests
    {
        [Fact]
        public void GetChildMapWhenParentIsMappedBeforeIt()
        {
            var parentMap = new ParentMap();
            var childMap = new ChildMap();
            var c = new CsvClassMapCollection();
            c.Add(parentMap);
            c.Add(childMap);

            var map = c[typeof(Child)];
            Assert.Equal(childMap, map);
        }

        private class Parent {}

        private class Child : Parent {}

        private sealed class ParentMap : CsvClassMap<Parent> {}

        private sealed class ChildMap : CsvClassMap<Child> {}
    }
}