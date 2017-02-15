namespace CsvHelper.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using CsvHelper.Configuration;

    public class SerializerMock : ICsvSerializer
    {
        private readonly bool throwExceptionOnWrite;

        public TextWriter TextWriter { get; }

        public ICsvSerializerConfiguration Configuration { get; }

        public List<string[]> Records { get; } = new List<string[]>();

        public SerializerMock(bool throwExceptionOnWrite = false)
        {
            Configuration = new CsvConfiguration();
            this.throwExceptionOnWrite = throwExceptionOnWrite;
        }

        public void Write(string[] record)
        {
            if (throwExceptionOnWrite)
            {
                throw new Exception("Mock Write exception.");
            }

            Records.Add(record);
        }

        public void Dispose() {}
    }
}