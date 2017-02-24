namespace CsvHelper.Tests.Mappings
{
    using System.IO;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;
    using Xunit;

    public class AnnotatedMappingTests
    {
        [Fact]
        public void MainTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("id,item_id,name,model_type,validity,title,desc");

                writer.WriteLine(@"1,ID101,TestSet,TR_proton57,yup,""122.title~1972"",125with some text200and stuff...");
                writer.WriteLine("2,ID122,TestSet,TR_neutron59,nope,123.title without suffix,test2999999----22");
                writer.WriteLine("3,IE122,TestSet,TT_proton59,nope,123.title without suffix,test2999999----22");

                writer.Flush();
                stream.Position = 0;

                csv.Configuration.RegisterClass<AnnotatedClass>();

                csv.Read();
                var record = csv.GetRecord<AnnotatedClass>();

                Assert.Equal(101, record.Id);
                Assert.Equal("TR_proton", record.Type);
                Assert.True(record.IsValid);
                Assert.Equal("title", record.Name);
                Assert.Equal("125.200", record.Desription);

                csv.Read();
                record = csv.GetRecord<AnnotatedClass>();

                Assert.Equal(122, record.Id);
                Assert.Equal("TR_neutron", record.Type);
                Assert.False(record.IsValid);
                Assert.Equal("title without suffix", record.Name);
                Assert.Equal("2999999.22", record.Desription);

                csv.Read();
                Assert.Throws<CsvBadDataException>(() => csv.GetRecord<AnnotatedClass>());
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private class AnnotatedClass
        {
            [CsvField("item_id", Prefix = "ID", IsPrefixMandatory = true, TrimPrefix = true)]
            public int Id { get; set; }

            [CsvField("title", TrimStart = 4, Suffix = "~1972", TrimSuffix = true)]
            public string Name { get; set; }

            [CsvField("model_type", Prefix = "TR_", TrimEnd = 2)]
            public string Type { get; set; }

            [CsvField("validity", TypeConverter = typeof(CoolBooleanTypeConverter))]
            public bool IsValid { get; set; }

            [CsvField("desc", @"[^\d]*(\d+)[^\d]+(\d+)[^\d]*", "$1.$2")]
            public string Desription { get; set; }
        }
        // ReSharper enable UnusedAutoPropertyAccessor.Local

        private class CoolBooleanTypeConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, ICsvReaderRow row, CsvPropertyMapData propertyMapData)
            {
                return text == "yup";
            }
        }
    }
}