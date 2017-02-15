﻿namespace CsvHelper
{
    using System;
    using System.IO;
    using System.Text;
    using Configuration;

    /// <summary>
    /// Reads fields from a <see cref="TextReader" />.
    /// </summary>
    public class FieldReader : IDisposable
    {
        private readonly char[] buffer;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private StringBuilder rawRecord = new StringBuilder();

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private StringBuilder field = new StringBuilder();

        private int bufferPosition;

        private int fieldStartPosition;

        private int fieldEndPosition;

        private int rawRecordStartPosition;

        private int rawRecordEndPosition;

        private int charsRead;

        private bool disposed;

        /// <summary>
        /// Gets the character position.
        /// </summary>
        public long CharPosition { get; private set; }

        /// <summary>
        /// Gets the byte position.
        /// </summary>
        public long BytePosition { get; private set; }

        /// <summary>
        /// Gets all the characters of the record including
        /// quotes, delimeters, and line endings.
        /// </summary>
        public string RawRecord => rawRecord.ToString();

        /// <summary>
        /// Gets the <see cref="TextReader" /> that is read from.
        /// </summary>
        public TextReader Reader { get; private set; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public ICsvParserConfiguration Configuration { get; }

        /// <summary>
        /// Gets or sets a value indicating if the field is bad.
        /// True if the field is bad, otherwise false.
        /// </summary>
        public bool IsFieldBad { get; set; }

        /// <summary>
        /// Creates a new <see cref="FieldReader" /> using the given
        /// <see cref="TextReader" /> and <see cref="CsvConfiguration" />.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="configuration"></param>
        public FieldReader(TextReader reader, ICsvParserConfiguration configuration)
        {
            Reader = reader;
            buffer = new char[configuration.BufferSize];
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the next char as an <see cref="int" />.
        /// </summary>
        /// <returns></returns>
        public virtual int GetChar()
        {
            if (bufferPosition >= charsRead)
            {
                if (Configuration.CountBytes)
                {
                    BytePosition += Configuration.Encoding.GetByteCount(buffer, rawRecordStartPosition, rawRecordEndPosition - rawRecordStartPosition);
                }

                rawRecord.Append(new string(buffer, rawRecordStartPosition, bufferPosition - rawRecordStartPosition));
                rawRecordStartPosition = 0;

                if (fieldEndPosition <= fieldStartPosition)
                {
                    // If the end position hasn't been set yet, use the buffer position instead.
                    fieldEndPosition = bufferPosition;
                }

                field.Append(new string(buffer, fieldStartPosition, fieldEndPosition - fieldStartPosition));
                bufferPosition = 0;
                rawRecordEndPosition = 0;
                fieldStartPosition = 0;
                fieldEndPosition = 0;

                charsRead = Reader.Read(buffer, 0, buffer.Length);
                if (charsRead == 0)
                {
                    // End of file.

                    // Clear out the buffer in case the stream
                    // is written to again and we need to read some more.
                    for (var i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = '\0';
                    }

                    return -1;
                }
            }

            var c = buffer[bufferPosition];
            bufferPosition++;
            rawRecordEndPosition = bufferPosition;

            CharPosition++;

            return c;
        }

        /// <summary>
        /// Gets the field. This will append any reading progress.
        /// </summary>
        /// <returns>The current field.</returns>
        public virtual string GetField()
        {
            AppendField();

            if (IsFieldBad && Configuration.ThrowOnBadData)
            {
                throw new CsvBadDataException($"Field: '{field}'");
            }

            if (IsFieldBad)
            {
                Configuration.BadDataCallback?.Invoke(field.ToString());
            }

            IsFieldBad = false;

            if (Configuration.UnescapeQuotes)
            {
                field.Replace("\\\"", "\"").Replace("\\'", "'");
            }

            var result = field.ToString();

            if (Configuration.IgnoreNullFields && result.Trim().Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                result = null;
            }

            field.Clear();

            return result;
        }

        /// <summary>
        /// Appends the current reading progress.
        /// </summary>
        public virtual void AppendField()
        {
            if (Configuration.CountBytes)
            {
                BytePosition += Configuration.Encoding.GetByteCount(buffer, rawRecordStartPosition, bufferPosition - rawRecordStartPosition);
            }

            rawRecord.Append(new string(buffer, rawRecordStartPosition, rawRecordEndPosition - rawRecordStartPosition));
            rawRecordStartPosition = rawRecordEndPosition;

            var length = fieldEndPosition - fieldStartPosition;
            field.Append(new string(buffer, fieldStartPosition, length));
            fieldStartPosition = bufferPosition;
            fieldEndPosition = 0;
        }

        /// <summary>
        /// Sets the start of the field to the current buffer position.
        /// </summary>
        /// <param name="offset">
        /// An offset for the field start.
        /// The offset should be less than 1.
        /// </param>
        public virtual void SetFieldStart(int offset = 0)
        {
            var position = bufferPosition + offset;
            if (position >= 0)
            {
                fieldStartPosition = position;
            }
        }

        /// <summary>
        /// Sets the end of the field to the current buffer position.
        /// </summary>
        /// <param name="offset">
        /// An offset for the field start.
        /// The offset should be less than 1.
        /// </param>
        public virtual void SetFieldEnd(int offset = 0)
        {
            var position = bufferPosition + offset;
            if (position >= 0)
            {
                fieldEndPosition = position;
            }
        }

        /// <summary>
        /// Sets the raw record end to the current buffer position.
        /// </summary>
        /// <param name="offset">
        /// An offset for the raw record end.
        /// The offset should be less than 1.
        /// </param>
        public virtual void SetRawRecordEnd(int offset)
        {
            var position = bufferPosition + offset;
            if (position >= 0)
            {
                rawRecordEndPosition = position;
            }
        }

        /// <summary>
        /// Clears the raw record.
        /// </summary>
        public virtual void ClearRawRecord()
        {
#if NET_2_0 || NET_3_5
			rawRecord = new StringBuilder();
#else
            rawRecord.Clear();
#endif
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if the instance needs to be disposed of.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                Reader?.Dispose();
            }

            disposed = true;
            Reader = null;
        }
    }
}