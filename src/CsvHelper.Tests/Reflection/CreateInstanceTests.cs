// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.Tests.Reflection
{
    using Xunit;

    public class ReflectionHelperTests
    {
        [Fact]
        public void CreateInstanceTests()
        {
            var test = ReflectionHelper.CreateInstance<Test>();

            Assert.NotNull(test);
            Assert.Equal("name", Test.Name);

            test = (Test)ReflectionHelper.CreateInstance(typeof(Test));
            Assert.NotNull(test);
            Assert.Equal("name", Test.Name);
        }

        private class Test
        {
            public static string Name => "name";
        }
    }
}