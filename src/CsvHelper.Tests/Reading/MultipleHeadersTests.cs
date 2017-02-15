namespace CsvHelper.Tests.Reading
{
    using System.IO;
    using CsvHelper.Configuration;
    using Xunit;

    public class MultipleHeadersTests
    {
        [Fact]
        public void ReadWithoutMapTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("A,B");
                writer.WriteLine("1,one");
                writer.WriteLine("Y,Z");
                writer.WriteLine("two,2");
                writer.Flush();
                stream.Position = 0;

                csv.Read();
                csv.ReadHeader();
                csv.Read();

                Assert.Equal(1, csv.GetField<int>("A"));
                Assert.Equal("one", csv.GetField("B"));

                csv.Read();
                csv.ReadHeader();
                csv.Read();

                Assert.Equal("two", csv.GetField("Y"));
                Assert.Equal(2, csv.GetField<int>("Z"));
            }
        }

        [Fact]
        public void ReadWithMapTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("A,B");
                writer.WriteLine("1,one");
                writer.WriteLine("Y,Z");
                writer.WriteLine("two,2");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.RegisterClassMap<AlphaMap>();
                csv.Configuration.RegisterClassMap<OmegaMap>();

                csv.Read();
                csv.ReadHeader();

                csv.Read();
                var alphaRecord = csv.GetRecord<Alpha>();

                Assert.Equal(1, alphaRecord.A);
                Assert.Equal("one", alphaRecord.B);

                csv.Read();
                csv.ReadHeader();

                csv.Read();
                var omegaRecord = csv.GetRecord<Omega>();

                Assert.Equal("two", omegaRecord.Y);
                Assert.Equal(2, omegaRecord.Z);
            }
        }

        private class Alpha
        {
            public int A { get; set; }

            public string B { get; set; }
        }

        private class Omega
        {
            public string Y { get; set; }

            public int Z { get; set; }
        }

        private sealed class AlphaMap : CsvClassMap<Alpha>
        {
            public AlphaMap()
            {
                Map(m => m.A);
                Map(m => m.B);
            }
        }

        private sealed class OmegaMap : CsvClassMap<Omega>
        {
            public OmegaMap()
            {
                Map(m => m.Y);
                Map(m => m.Z);
            }
        }
    }
}