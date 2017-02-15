namespace CsvHelper.Tests.Writing
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using Xunit;

    public class DynamicTests
    {
        [Fact]
        public void WriteDynamicExpandoObjectsTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                var list = new List<dynamic>();
                dynamic obj = new ExpandoObject();
                obj.Id = 1;
                obj.Name = "one";
                list.Add(obj);

                obj = new ExpandoObject();
                obj.Id = 2;
                obj.Name = "two";
                list.Add(obj);

                csv.WriteRecords(list);
                writer.Flush();
                stream.Position = 0;

                var expected = "Id,Name\r\n";
                expected += "1,one\r\n";
                expected += "2,two\r\n";

                Assert.Equal(expected, reader.ReadToEnd());
            }
        }

        [Fact]
        public void WriteDynamicExpandoObjectTest()
        {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                dynamic obj = new ExpandoObject();
                obj.Id = 1;
                obj.Name = "one";

                csv.WriteRecord(obj);
                csv.NextRecord();

                obj = new ExpandoObject();
                obj.Id = 2;
                obj.Name = "two";

                csv.WriteRecord(obj);
                csv.NextRecord();

                writer.Flush();
                stream.Position = 0;

                var expected = "Id,Name\r\n";
                expected += "1,one\r\n";
                expected += "2,two\r\n";

                Assert.Equal(expected, reader.ReadToEnd());
            }
        }
    }
}