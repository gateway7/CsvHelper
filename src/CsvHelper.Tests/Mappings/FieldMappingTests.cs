﻿
#pragma warning disable 649

namespace CsvHelper.Tests.Mappings
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using CsvHelper.Configuration;
    using Xunit;

    public class FieldMappingTests
    {
        [Fact]
        public void ReadPublicFieldsWithAutoMapTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("IdField,NameField");
                writer.WriteLine("1,one");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.MemberTypes = MemberTypes.Fields;
                var records = csv.GetRecords<APublic>().ToList();

                Assert.Equal(1, records.Count);
                Assert.Equal(1, records[0].IdField);
                Assert.Equal("one", records[0].BField.NameField);
            }
        }

        [Fact]
        public void WritePublicFieldsWithAutoMapTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvWriter(writer))
            {
                var list = new List<APublic>
                {
                    new APublic
                    {
                        IdField = 1,
                        BField = new BPublic
                        {
                            NameField = "one"
                        }
                    }
                };
                csv.Configuration.MemberTypes = MemberTypes.Fields;
                csv.WriteRecords(list);
                writer.Flush();
                stream.Position = 0;

                var result = reader.ReadToEnd();

                var expected = new StringBuilder();
                expected.AppendLine("IdField,NameField");
                expected.AppendLine("1,one");

                Assert.Equal(expected.ToString(), result);
            }
        }

        [Fact]
        public void ReadPublicFieldsWithMappingTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("IdField,NameField");
                writer.WriteLine("1,one");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.RegisterClassMap<APublicMap>();
                var records = csv.GetRecords<APublic>().ToList();

                Assert.Equal(1, records.Count);
                Assert.Equal(1, records[0].IdField);
                Assert.Equal("one", records[0].BField.NameField);
            }
        }

        [Fact]
        public void WritePublicFieldsWithMappingTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvWriter(writer))
            {
                var list = new List<APublic>
                {
                    new APublic
                    {
                        IdField = 1,
                        BField = new BPublic
                        {
                            NameField = "one"
                        }
                    }
                };
                csv.Configuration.RegisterClassMap<APublicMap>();
                csv.WriteRecords(list);
                writer.Flush();
                stream.Position = 0;

                var result = reader.ReadToEnd();

                var expected = new StringBuilder();
                expected.AppendLine("IdField,NameField");
                expected.AppendLine("1,one");

                Assert.Equal(expected.ToString(), result);
            }
        }

        [Fact]
        public void ReadPrivateFieldsWithAutoMapTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("idField,nameField");
                writer.WriteLine("1,one");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.IncludePrivateMembers = true;
                csv.Configuration.MemberTypes = MemberTypes.Fields;
                var records = csv.GetRecords<APrivate>().ToList();

                Assert.Equal(1, records.Count);
                Assert.Equal(1, records[0].GetId());
                Assert.Equal("one", records[0].GetB().GetName());
            }
        }

        [Fact]
        public void WritePrivateFieldsWithAutoMapTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvWriter(writer))
            {
                var list = new List<APrivate>
                {
                    new APrivate(1, "one")
                };

                csv.Configuration.IncludePrivateMembers = true;
                csv.Configuration.MemberTypes = MemberTypes.Fields;
                csv.WriteRecords(list);
                writer.Flush();
                stream.Position = 0;

                var result = reader.ReadToEnd();

                var expected = new StringBuilder();
                expected.AppendLine("idField,nameField");
                expected.AppendLine("1,one");

                Assert.Equal(expected.ToString(), result);
            }
        }

        [Fact]
        public void ReadPrivatreFieldsWithMappingTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("idField,nameField");
                writer.WriteLine("1,one");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.RegisterClassMap<APrivateMap>();
                var records = csv.GetRecords<APrivate>().ToList();

                Assert.Equal(1, records.Count);
                Assert.Equal(1, records[0].GetId());
                Assert.Equal("one", records[0].GetB().GetName());
            }
        }

        [Fact]
        public void WritePrivateFieldsWithMappingTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvWriter(writer))
            {
                var list = new List<APrivate>
                {
                    new APrivate(1, "one")
                };
                csv.Configuration.RegisterClassMap<APrivateMap>();
                csv.WriteRecords(list);
                writer.Flush();
                stream.Position = 0;

                var result = reader.ReadToEnd();

                var expected = new StringBuilder();
                expected.AppendLine("idField,nameField");
                expected.AppendLine("1,one");

                Assert.Equal(expected.ToString(), result);
            }
        }

        [Fact]
        public void ReadPublicFieldsAndPropertiesWithAutoMapTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                writer.WriteLine("IdField,NameField,IdProp,NameProp");
                writer.WriteLine("1,one,2,two");
                writer.Flush();
                stream.Position = 0;

                csv.Configuration.MemberTypes = MemberTypes.Properties | MemberTypes.Fields;
                var records = csv.GetRecords<APublic>().ToList();

                Assert.Equal(1, records.Count);
                Assert.Equal(1, records[0].IdField);
                Assert.Equal("one", records[0].BField.NameField);
            }
        }

        [Fact]
        public void WritePublicFieldsAndPropertiesWithAutoMapTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvWriter(writer))
            {
                var list = new List<APublic>
                {
                    new APublic
                    {
                        IdField = 1,
                        BField = new BPublic
                        {
                            NameField = "one",
                            NameProp = "two"
                        },
                        IdProp = 2
                    }
                };
                csv.Configuration.MemberTypes = MemberTypes.Properties | MemberTypes.Fields;
                csv.WriteRecords(list);
                writer.Flush();
                stream.Position = 0;

                var result = reader.ReadToEnd();

                var expected = new StringBuilder();
                expected.AppendLine("IdProp,IdField,NameProp,NameField");
                expected.AppendLine("2,1,two,one");

                Assert.Equal(expected.ToString(), result);
            }
        }

        private class APublic
        {
            public int IdField;

            public BPublic BField;

            public int IdProp { get; set; }
        }

        private class BPublic
        {
            public string NameField;

            public string NameProp { get; set; }
        }

        private sealed class APublicMap : CsvClassMap<APublic>
        {
            public APublicMap()
            {
                Map(m => m.IdField);
                References<BPublicMap>(m => m.BField);
            }
        }

        private sealed class BPublicMap : CsvClassMap<BPublic>
        {
            public BPublicMap()
            {
                Map(m => m.NameField);
            }
        }

        private class APrivate
        {
            private readonly int idField;

            private readonly BPrivate bField;

            public int GetId()
            {
                return idField;
            }

            public BPrivate GetB()
            {
                return bField;
            }

            public APrivate() {}

            public APrivate(int id, string name)
            {
                idField = id;
                bField = new BPrivate(name);
            }

            private int IdProp { get; set; }

            private BPrivate BProp { get; set; }
        }

        private class BPrivate
        {
            private readonly string nameField;

            public string GetName()
            {
                return nameField;
            }

            public BPrivate() {}

            public BPrivate(string name)
            {
                nameField = name;
            }
        }

        private sealed class APrivateMap : CsvClassMap<APrivate>
        {
            public APrivateMap()
            {
                var options = new AutoMapOptions
                {
                    IncludePrivateProperties = true,
                    MemberTypes = MemberTypes.Fields
                };
                AutoMap(options);
            }
        }
    }
}