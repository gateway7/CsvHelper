﻿// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.Tests
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;
    using Xunit;

    public class CsvReaderErrorMessageTests
    {
        public CsvReaderErrorMessageTests()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        }

        [Fact]
        public void FirstColumnEmptyFirstRowErrorWithNoHeaderTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csvReader = new CsvReader(reader))
            {
                csvReader.Configuration.HasHeaderRecord = false;
                csvReader.Configuration.AllowComments = true;
                csvReader.Configuration.RegisterClassMap<Test1Map>();
                writer.WriteLine(",one");
                writer.WriteLine("2,two");
                writer.Flush();
                stream.Position = 0;

                try
                {
                    var records = csvReader.GetRecords<Test1>().ToList();
                    throw new Exception();
                }
                catch (CsvTypeConverterException ex)
                {
                    Assert.Equal(1, ex.Row);
                    Assert.Equal(0, ex.FieldIndex);
                }
            }
        }

        [Fact]
        public void FirstColumnEmptySecondRowErrorWithHeader()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csvReader = new CsvReader(reader))
            {
                csvReader.Configuration.AllowComments = true;
                csvReader.Configuration.RegisterClassMap<Test1Map>();
                writer.WriteLine("IntColumn,StringColumn");
                writer.WriteLine("1,one");
                writer.WriteLine(",two");
                writer.Flush();
                stream.Position = 0;

                try
                {
                    var records = csvReader.GetRecords<Test1>().ToList();
                    throw new Exception();
                }
                catch (CsvTypeConverterException ex)
                {
                    Assert.Equal(3, ex.Row);
                    Assert.Equal(0, ex.FieldIndex);
                }
            }
        }

        [Fact]
        public void FirstColumnEmptyErrorWithHeaderAndCommentRowTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csvReader = new CsvReader(reader))
            {
                csvReader.Configuration.AllowComments = true;
                csvReader.Configuration.RegisterClassMap<Test1Map>();
                writer.WriteLine("IntColumn,StringColumn");
                writer.WriteLine("# comment");
                writer.WriteLine();
                writer.WriteLine(",one");
                writer.WriteLine("2,two");
                writer.Flush();
                stream.Position = 0;

                try
                {
                    var records = csvReader.GetRecords<Test1>().ToList();
                    throw new Exception();
                }
                catch (CsvTypeConverterException ex)
                {
                    Assert.Equal(4, ex.Row);
                    Assert.Equal(0, ex.FieldIndex);
                }
            }
        }

        [Fact]
        public void FirstColumnErrorTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csvReader = new CsvReader(reader))
            {
                csvReader.Configuration.RegisterClassMap<Test1Map>();
                writer.WriteLine("IntColumn,StringColumn");
                writer.WriteLine();
                writer.WriteLine("one,one");
                writer.WriteLine("2,two");
                writer.Flush();
                stream.Position = 0;

                try
                {
                    var records = csvReader.GetRecords<Test1>().ToList();
                    throw new Exception();
                }
                catch (CsvTypeConverterException ex)
                {
                    Assert.Equal(3, ex.Row);
                    Assert.Equal(0, ex.FieldIndex);
                }
            }
        }

        [Fact]
        public void SecondColumnEmptyErrorTest()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csvReader = new CsvReader(reader))
            {
                csvReader.Configuration.RegisterClassMap<Test2Map>();
                writer.WriteLine("StringColumn,IntColumn");
                writer.WriteLine("one,");
                writer.WriteLine("two,2");
                writer.Flush();
                stream.Position = 0;

                try
                {
                    var records = csvReader.GetRecords<Test2>().ToList();
                    throw new Exception();
                }
                catch (CsvTypeConverterException ex)
                {
                    Assert.Equal(2, ex.Row);
                    Assert.Equal(1, ex.FieldIndex);
                }
            }
        }

        [Fact]
        public void Test()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            using (var csvReader = new CsvReader(reader))
            {
                writer.WriteLine("1,9/24/2012");
                writer.Flush();
                stream.Position = 0;

                try
                {
                    csvReader.Configuration.HasHeaderRecord = false;
                    csvReader.Configuration.RegisterClassMap<Test3Map>();
                    var records = csvReader.GetRecords<Test3>().ToList();
                }
                catch (CsvReaderException)
                {
                    // Should throw this exception.
                }
            }
        }

        private class Test1
        {
            public int IntColumn { get; set; }

            public string StringColumn { get; set; }
        }

        private sealed class Test1Map : CsvClassMap<Test1>
        {
            public Test1Map()
            {
                Map(m => m.IntColumn).Index(0);
                Map(m => m.StringColumn).Index(1);
            }
        }

        private class Test2
        {
            public string StringColumn { get; set; }

            public int IntColumn { get; set; }
        }

        private sealed class Test2Map : CsvClassMap<Test2>
        {
            public Test2Map()
            {
                Map(m => m.StringColumn);
                Map(m => m.IntColumn);
            }
        }

        private class Test3
        {
            public int Id { get; set; }

            public DateTime CreationDate { get; set; }

            public string Description { get; set; }
        }

        private sealed class Test3Map : CsvClassMap<Test3>
        {
            public Test3Map()
            {
                Map(m => m.Id).Index(0);
                Map(m => m.CreationDate).Index(1);
                Map(m => m.Description).Index(2);
            }
        }
    }
}