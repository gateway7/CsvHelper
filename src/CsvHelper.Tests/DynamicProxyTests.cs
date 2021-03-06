﻿// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Castle.DynamicProxy;
    using CsvHelper.Configuration;
    using Xunit;

    public class DynamicProxyTests
    {
        [Fact]
        public void WriteDynamicProxyObjectTest()
        {
            var list = new List<TestClass>();
            var proxyGenerator = new ProxyGenerator();
            for (var i = 0; i < 1; i++)
            {
                var proxy = proxyGenerator.CreateClassProxy<TestClass>();
                proxy.Id = i + 1;
                proxy.Name = "name" + proxy.Id;
                list.Add(proxy);
            }

            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.RegisterClassMap<TestClassMap>();
                csv.WriteRecords(list);
                writer.Flush();
                stream.Position = 0;

                var data = reader.ReadToEnd();
                var expected = new StringBuilder();
                expected.AppendLine("id,name");
                expected.AppendLine("1,name1");

                Assert.Equal(expected.ToString(), data);
            }
        }

        public class TestClass
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        private sealed class TestClassMap : CsvClassMap<TestClass>
        {
            public TestClassMap()
            {
                Map(m => m.Id).Name("id");
                Map(m => m.Name).Name("name");
            }
        }
    }
}