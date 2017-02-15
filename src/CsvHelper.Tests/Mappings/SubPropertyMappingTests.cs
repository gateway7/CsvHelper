namespace CsvHelper.Tests.Mappings
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CsvHelper.Configuration;
    using Xunit;

    public class SubPropertyMappingTests
    {
        [Fact]
        public void ReadTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("P3,P1,P2");
                writer.WriteLine("p3,p1,p2");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.RegisterClassMap<AMap>();
                var records = csv.GetRecords<A>().ToList();

                Assert.Equal("p1", records[0].P1);
                Assert.Equal("p2", records[0].B.P2);
                Assert.Equal("p3", records[0].B.C.P3);
            }
        }

        [Fact]
        public void WriteTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                var list = new List<A>
                {
                    new A
                    {
                        P1 = "p1",
                        B = new B
                        {
                            P2 = "p2",
                            C = new C
                            {
                                P3 = "p3"
                            }
                        }
                    }
                };

                csv.Configuration.RegisterClassMap<AMap>();
                csv.WriteRecords(list);
                writer.Flush();
                stream.Position = 0;

                var expected = "P3,P1,P2\r\n";
                expected += "p3,p1,p2\r\n";
                var result = reader.ReadToEnd();

                Assert.Equal(expected, result);
            }
        }

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
        }

        private sealed class AMap : CsvClassMap<A>
        {
            public AMap()
            {
                Map(m => m.B.C.P3).Index(0);
                Map(m => m.P1).Index(1);
                Map(m => m.B.P2).Index(2);
            }
        }
    }
}