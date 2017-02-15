namespace CsvHelper.Tests.Mappings
{
    using System.IO;
    using System.Linq;
    using CsvHelper.Configuration;
    using Xunit;

    public class ConstructUsingTests
    {
        [Fact]
        public void ConstructUsingNewTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("1,2,3");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.RegisterClassMap<ANewMap>();
                var records = csv.GetRecords<A>().ToList();
                var record = records[0];

                Assert.Equal("a name", record.Name);
                Assert.Equal("b name", record.B.Name);
            }
        }

        [Fact]
        public void ConstructUsingMemberInitTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("1,2,3");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.RegisterClassMap<AMemberInitMap>();
                var records = csv.GetRecords<A>().ToList();
                var record = records[0];

                Assert.Equal("a name", record.Name);
                Assert.Equal("b name", record.B.Name);
            }
        }

        public class B
        {
            public string Name { get; set; }

            public B() {}

            public B(string name)
            {
                Name = name;
            }
        }

        private class A
        {
            public string Name { get; set; }

            public B B { get; set; }

            public A() {}

            public A(string name)
            {
                Name = name;
            }
        }

        private sealed class ANewMap : CsvClassMap<A>
        {
            public ANewMap()
            {
                ConstructUsing(() => new A("a name"));
                References<BNewMap>(m => m.B);
            }
        }

        private sealed class BNewMap : CsvClassMap<B>
        {
            public BNewMap()
            {
                ConstructUsing(() => new B("b name"));
            }
        }

        private sealed class AMemberInitMap : CsvClassMap<A>
        {
            public AMemberInitMap()
            {
                ConstructUsing(() => new A { Name = "a name" });
                References<BMemberInitMap>(m => m.B);
            }
        }

        private sealed class BMemberInitMap : CsvClassMap<B>
        {
            public BMemberInitMap()
            {
                ConstructUsing(() => new B { Name = "b name" });
            }
        }
    }
}