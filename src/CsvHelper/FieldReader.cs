namespace CsvHelper
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
        private readonly char[] _buffer;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private StringBuilder _rawRecord = new StringBuilder();

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private StringBuilder _field = new StringBuilder();

        private int _bufferPosition;

        private int _fieldStartPosition;

        private int _fieldEndPosition;

        private int _rawRecordStartPosition;

        private int _rawRecordEndPosition;

        private int _charsRead;

        private bool _disposed;

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
        /// quotes, delimiters, and line endings.
        /// </summary>
        public string RawRecord => _rawRecord.ToString();

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
            _buffer = new char[configuration.BufferSize];
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the next char as an <see cref="int" />.
        /// </summary>
        /// <returns></returns>
        public virtual int GetChar()
        {
            if (_bufferPosition >= _charsRead)
            {
                if (Configuration.CountBytes)
                {
                    BytePosition += Configuration.Encoding.GetByteCount(_buffer, _rawRecordStartPosition, _rawRecordEndPosition - _rawRecordStartPosition);
                }

                _rawRecord.Append(new string(_buffer, _rawRecordStartPosition, _bufferPosition - _rawRecordStartPosition));
                _rawRecordStartPosition = 0;

                if (_fieldEndPosition <= _fieldStartPosition)
                {
                    // If the end position hasn't been set yet, use the buffer position instead.
                    _fieldEndPosition = _bufferPosition;
                }

                _field.Append(new string(_buffer, _fieldStartPosition, _fieldEndPosition - _fieldStartPosition));
                _bufferPosition = 0;
                _rawRecordEndPosition = 0;
                _fieldStartPosition = 0;
                _fieldEndPosition = 0;

                _charsRead = Reader.Read(_buffer, 0, _buffer.Length);
                if (_charsRead == 0)
                {
                    // End of file.

                    // Clear out the buffer in case the stream
                    // is written to again and we need to read some more.
                    for (var i = 0; i < _buffer.Length; i++)
                    {
                        _buffer[i] = '\0';
                    }

                    return -1;
                }
            }

            var c = _buffer[_bufferPosition];
            _bufferPosition++;
            _rawRecordEndPosition = _bufferPosition;

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
                throw new CsvBadDataException($"Field: '{_field}'");
            }

            if (IsFieldBad)
            {
                Configuration.BadDataCallback?.Invoke(_field.ToString());
            }

            IsFieldBad = false;

            if (Configuration.UnescapeQuotes)
            {
                _field.Replace("\\\"", "\"").Replace("\\'", "'");
            }

            var result = _field.ToString();

            _field.Clear();

            return result;
        }

        /// <summary>
        /// Appends the current reading progress.
        /// </summary>
        public virtual void AppendField()
        {
            if (Configuration.CountBytes)
            {
                BytePosition += Configuration.Encoding.GetByteCount(_buffer, _rawRecordStartPosition, _bufferPosition - _rawRecordStartPosition);
            }

            _rawRecord.Append(new string(_buffer, _rawRecordStartPosition, _rawRecordEndPosition - _rawRecordStartPosition));
            _rawRecordStartPosition = _rawRecordEndPosition;

            var length = _fieldEndPosition - _fieldStartPosition;
            _field.Append(new string(_buffer, _fieldStartPosition, length));
            _fieldStartPosition = _bufferPosition;
            _fieldEndPosition = 0;
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
            var position = _bufferPosition + offset;
            if (position >= 0)
            {
                _fieldStartPosition = position;
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
            var position = _bufferPosition + offset;
            if (position >= 0)
            {
                _fieldEndPosition = position;
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
            var position = _bufferPosition + offset;
            if (position >= 0)
            {
                _rawRecordEndPosition = position;
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
            _rawRecord.Clear();
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
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Reader?.Dispose();
            }

            _disposed = true;
            Reader = null;
        }
    }
}