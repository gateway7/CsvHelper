﻿namespace CsvHelper
{
    using System;
    using System.IO;
    using System.Linq;
    using Configuration;

    /// <summary>
    /// Parses a CSV file.
    /// </summary>
    public class CsvParser : ICsvParser
    {
        private readonly bool _leaveOpen;

        private readonly RecordBuilder _recordBuilder = new RecordBuilder();

        private readonly ICsvParserConfiguration _configuration;

        private FieldReader _fieldReader;

        private bool _isDisposed;

        private int _currentRow;

        private int _currentRawRow;

        private int _currentCharacter = -1;

        private bool _hasExcelSeparatorBeenRead;

        /// <summary>
        /// Gets the <see cref="ICsvParser.TextReader" />.
        /// </summary>
        public virtual TextReader TextReader => _fieldReader.Reader;

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public virtual ICsvParserConfiguration Configuration => _configuration;

        /// <summary>
        /// Gets the character position that the parser is currently on.
        /// </summary>
        public virtual long CharPosition => _fieldReader.CharPosition;

        /// <summary>
        /// Gets the byte position that the parser is currently on.
        /// </summary>
        public virtual long BytePosition => _fieldReader.BytePosition;

        /// <summary>
        /// Gets the row of the CSV file that the parser is currently on.
        /// </summary>
        public virtual int Row => _currentRow;

        /// <summary>
        /// Gets the row of the CSV file that the parser is currently on.
        /// This is the actual file row.
        /// </summary>
        public virtual int RawRow => _currentRawRow;

        /// <summary>
        /// Gets the raw row for the current record that was parsed.
        /// </summary>
        public virtual string RawRecord => _fieldReader.RawRecord;

        /// <summary>
        /// Creates a new parser using the given <see cref="TextReader" />.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader" /> with the CSV file data.</param>
        public CsvParser(TextReader reader) : this(reader, new CsvConfiguration(), false) { }

        /// <summary>
        /// Creates a new parser using the given <see cref="TextReader" />.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader" /> with the CSV file data.</param>
        /// <param name="leaveOpen">true to leave the reader open after the CsvReader object is disposed, otherwise false.</param>
        public CsvParser(TextReader reader, bool leaveOpen) : this(reader, new CsvConfiguration(), false) { }

        /// <summary>
        /// Creates a new parser using the given <see cref="TextReader" /> and <see cref="CsvConfiguration" />.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader" /> with the CSV file data.</param>
        /// <param name="configuration">The configuration.</param>
        public CsvParser(TextReader reader, ICsvParserConfiguration configuration) : this(reader, configuration, false) { }

        /// <summary>
        /// Creates a new parser using the given <see cref="TextReader" /> and <see cref="CsvConfiguration" />.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader" /> with the CSV file data.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="leaveOpen">true to leave the reader open after the CsvReader object is disposed, otherwise false.</param>
        public CsvParser(TextReader reader, ICsvParserConfiguration configuration, bool leaveOpen)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _fieldReader = new FieldReader(reader, configuration);
            _configuration = configuration;
            _leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Reads a record from the CSV file.
        /// </summary>
        /// <returns>A <see cref="T:String[]" /> of fields for the record read.</returns>
        public virtual string[] Read()
        {
            try
            {
                if (_configuration.HasExcelSeparator && !_hasExcelSeparatorBeenRead)
                {
                    ReadExcelSeparator();
                }

                _fieldReader.ClearRawRecord();

                var row = ReadLine();

                return row;
            }
            catch (Exception ex)
            {
                var csvHelperException = ex as CsvHelperException ?? new CsvParserException("An unexpected error occurred.", ex);
                ExceptionHelper.AddExceptionData(csvHelperException, Row, null, null, null, _recordBuilder.ToArray());

                throw csvHelperException;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            Dispose(!_leaveOpen);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if the instance needs to be disposed of.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _fieldReader?.Dispose();
            }

            _isDisposed = true;
            _fieldReader = null;
        }

        /// <summary>
        /// Reads a line of the CSV file.
        /// </summary>
        /// <returns>The CSV line.</returns>
        protected virtual string[] ReadLine()
        {
            _recordBuilder.Clear();
            _currentRow++;
            _currentRawRow++;

            while (true)
            {
                var previousCharacter = _currentCharacter;
                _currentCharacter = _fieldReader.GetChar();

                if (_currentCharacter == -1)
                {
                    // We have reached the end of the file.
                    if (_recordBuilder.Length > 0)
                    {
                        // There was no line break at the end of the file.
                        // We need to return the last record first.
                        _recordBuilder.Add(_fieldReader.GetField());
                        return _recordBuilder.ToArray();
                    }

                    return null;
                }

                if (_configuration.UseExcelLeadingZerosFormatForNumerics)
                {
                    if (ReadExcelLeadingZerosField())
                    {
                        break;
                    }

                    continue;
                }

                if (_recordBuilder.Length == 0 && (_currentCharacter == _configuration.Comment && _configuration.AllowComments || IsEndOfLine()))
                {
                    ReadBlankLine();
                    if (!_configuration.IgnoreBlankLines)
                    {
                        break;
                    }

                    continue;
                }

                if (IsQuote() && previousCharacter != '\\' && !_configuration.IgnoreQuotes)
                {
                    if (ReadQuotedField())
                    {
                        break;
                    }
                }
                else
                {
                    if (ReadField())
                    {
                        break;
                    }
                }
            }

            return _recordBuilder.ToArray();
        }

        /// <summary>
        /// Reads a blank line. This accounts for empty lines
        /// and commented out lines.
        /// </summary>
        protected virtual void ReadBlankLine()
        {
            if (_configuration.IgnoreBlankLines)
            {
                _currentRow++;
            }

            while (true)
            {
                if (_currentCharacter == '\r' || _currentCharacter == '\n')
                {
                    ReadLineEnding();
                    _fieldReader.SetFieldStart();
                    return;
                }

                if (_currentCharacter == -1)
                {
                    return;
                }

                // If the buffer runs, it appends the current data to the field.
                // We don't want to capture any data on a blank line, so we
                // need to set the field start every char.
                _fieldReader.SetFieldStart();
                _currentCharacter = _fieldReader.GetChar();
            }
        }

        /// <summary>
        /// Reads until a delimiter or line ending is found.
        /// </summary>
        /// <returns>True if the end of the line was found, otherwise false.</returns>
        protected virtual bool ReadField()
        {
            if (!IsDelimiter() && !IsEndOfLine())
            {
                _currentCharacter = _fieldReader.GetChar();
            }

            while (true)
            {
                if (IsQuote())
                {
                    _fieldReader.IsFieldBad = true;
                }

                if (IsDelimiter())
                {
                    _fieldReader.SetFieldEnd(-1);

                    // End of field.
                    if (ReadDelimiter())
                    {
                        // Set the end of the field to the char before the delimiter.
                        _recordBuilder.Add(_fieldReader.GetField());
                        return false;
                    }
                }
                else if (IsEndOfLine())
                {
                    // End of line.
                    _fieldReader.SetFieldEnd(-1);
                    var offset = ReadLineEnding();
                    _fieldReader.SetRawRecordEnd(offset);
                    _recordBuilder.Add(_fieldReader.GetField());

                    _fieldReader.SetFieldStart(offset);

                    return true;
                }
                else if (_currentCharacter == -1)
                {
                    // End of file.
                    _fieldReader.SetFieldEnd();
                    _recordBuilder.Add(_fieldReader.GetField());
                    return true;
                }

                _currentCharacter = _fieldReader.GetChar();
            }
        }

        /// <summary>
        /// Reads until the field is not quoted and a delimiter is found.
        /// </summary>
        /// <returns>True if the end of the line was found, otherwise false.</returns>
        protected virtual bool ReadQuotedField()
        {
            var isInQuotes = true;

            // Set the start of the field to after the quote.
            _fieldReader.SetFieldStart();

            while (true)
            {
                // 1,"2" ,3

                var previousCharacter = _currentCharacter;
                _currentCharacter = _fieldReader.GetChar();

                if (IsQuote())
                {
                    if (IsBackslash(previousCharacter))
                    {
                        continue;
                    }

                    isInQuotes = !isInQuotes;

                    if (!isInQuotes)
                    {
                        // Add an offset for the quote.
                        _fieldReader.SetFieldEnd(-1);
                        _fieldReader.AppendField();
                        _fieldReader.SetFieldStart();
                    }

                    continue;
                }

                if (isInQuotes)
                {
                    // TODO: replace this ugliness with something more efficient and elegant! 
                    if (IsDelimiter() && IsQuote(previousCharacter) &&  IsBackslash(PeekFieldChar(-2)) && IsQuote(PeekFieldChar(-3)) && !IsBackslash(PeekFieldChar(-4)))
                    {
                        _fieldReader.SetFieldEnd(-2);
                        _recordBuilder.Add(_fieldReader.GetField());
                        return false;
                    }

                    if (IsEndOfLine())
                    {
                        ReadLineEnding();
                        _currentRawRow++;
                    }

                    if (_currentCharacter == -1)
                    {
                        _fieldReader.SetFieldEnd();
                        _recordBuilder.Add(_fieldReader.GetField());
                        return true;
                    }
                }

                if (!isInQuotes)
                {
                    if (IsDelimiter())
                    {
                        _fieldReader.SetFieldEnd(-1);

                        if (ReadDelimiter())
                        {
                            // Add an extra offset because of the end quote.
                            _recordBuilder.Add(_fieldReader.GetField());
                            return false;
                        }
                    }
                    else if (IsEndOfLine())
                    {
                        _fieldReader.SetFieldEnd(-1);
                        var offset = ReadLineEnding();
                        _fieldReader.SetRawRecordEnd(offset);
                        _recordBuilder.Add(_fieldReader.GetField());
                        _fieldReader.SetFieldStart(offset);
                        return true;
                    }
                    else if (previousCharacter == _configuration.Quote)
                    {
                        // We're out of quotes. Read the reset of
                        // the field like a normal field.
                        return ReadField();
                    }
                }
            }
        }

        /// <summary>
        /// Reads the field using Excel leading zero compatibility.
        /// i.e. Fields that start with `=`.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ReadExcelLeadingZerosField()
        {
            if (_currentCharacter == '=')
            {
                _currentCharacter = _fieldReader.GetChar();
                if (IsQuote() && !_configuration.IgnoreQuotes)
                {
                    // This is a valid Excel formula.
                    return ReadQuotedField();
                }
            }

            // The format is invalid.
            // Excel isn't consistent, so just read as normal.

            if (IsQuote() && !_configuration.IgnoreQuotes)
            {
                return ReadQuotedField();
            }

            return ReadField();
        }

        /// <summary>
        /// Reads until the delimiter is done.
        /// </summary>
        /// <returns>
        /// True if a delimiter was read. False if the sequence of
        /// chars ended up not being the delimiter.
        /// </returns>
        protected virtual bool ReadDelimiter()
        {
            if (!IsDelimiter())
            {
                throw new InvalidOperationException("Tried reading a delimiter when the first delimiter char didn't match the current char.");
            }

            if (_configuration.Delimiter.Length == 1)
            {
                return true;
            }

            for (var i = 1; i < _configuration.Delimiter.Length; i++)
            {
                _currentCharacter = _fieldReader.GetChar();
                if (_currentCharacter != _configuration.Delimiter[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads until the line ending is done.
        /// </summary>
        /// <returns>True if more chars were read, otherwise false.</returns>
        protected virtual int ReadLineEnding()
        {
            var fieldStartOffset = 0;
            if (_currentCharacter == '\r')
            {
                _currentCharacter = _fieldReader.GetChar();
                if (_currentCharacter != '\n')
                {
                    // The start needs to be moved back.
                    fieldStartOffset--;
                }
            }

            return fieldStartOffset;
        }

        /// <summary>
        /// Reads the Excel separator and sets it to the delimiter.
        /// </summary>
        protected virtual void ReadExcelSeparator()
        {
            // sep=delimiter
            var sepLine = _fieldReader.Reader.ReadLine();
            if (sepLine != null)
            {
                _configuration.Delimiter = sepLine.Substring(4);
            }

            _hasExcelSeparatorBeenRead = true;
        }

        private bool IsDelimiter(int character)
        {
            return Configuration.Delimiter.Any(p => p == character);
        }

        private bool IsDelimiter()
        {
            return IsDelimiter(_currentCharacter);
        }

        private bool IsEndOfLine()
        {
            return _currentCharacter == '\r' || _currentCharacter == '\n';
        }

        private bool IsQuote(int character)
        {
            return character == _configuration.Quote;
        }

        private bool IsQuote()
        {
            return IsQuote(_currentCharacter);
        }

        private bool IsBackslash(int character)
        {
            return character == '\\';
        }

        private int PeekFieldChar(int offset)
        {
            // _currentCharacter is at _fieldReader[-1]
            return _fieldReader.PeekChar(offset - 1);
        }
    }
}