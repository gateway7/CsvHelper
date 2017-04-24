// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.Configuration
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using TypeConversion;

    /// <summary>
    /// Configuration used for reading and writing CSV data.
    /// </summary>
    public class CsvConfiguration : ICsvReaderConfiguration, ICsvWriterConfiguration
    {
        private readonly CsvClassMapCollection _maps = new CsvClassMapCollection();

        private string _delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

        private char _quote = '"';

        private string _quoteString = "\"";

        private string _doubleQuoteString = "\"\"";

        private char[] _quoteRequiredChars;

        private CultureInfo _cultureInfo = CultureInfo.CurrentCulture;

        private bool _quoteAllFields;

        private bool _quoteNoFields;

        /// <summary>
        /// Gets or sets the <see cref="TypeConverterOptions" />.
        /// </summary>
        public virtual TypeConverterOptionsCollection TypeConverterOptions { get; set; } = new TypeConverterOptionsCollection();

        /// <summary>
        /// Gets or sets the property/field binding flags.
        /// This determines what properties/fields on the custom
        /// class are used. Default is Public | Instance.
        /// </summary>
        public virtual BindingFlags PropertyBindingFlags { get; set; } = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// Gets or sets a value indicating if the
        /// CSV file has a header record.
        /// Default is true.
        /// </summary>
        public virtual bool HasHeaderRecord { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating the if the CSV
        /// file contains the Excel "sep=delimiter" config
        /// option in the first row.
        /// </summary>
        public virtual bool HasExcelSeparator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if an exception will be
        /// thrown if a field defined in a mapping is missing.
        /// True to throw an exception, otherwise false.
        /// Default is true.
        /// </summary>
        public virtual bool WillThrowOnMissingField { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether changes in the column
        /// count should be detected. If true, a <see cref="CsvBadDataException" />
        /// will be thrown if a different column count is detected.
        /// </summary>
        /// <value>
        /// <c>true</c> if [detect column count changes]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool DetectColumnCountChanges { get; set; }

        /// <summary>
        /// Prepares the header field for matching against a property/field name.
        /// The header field and the property/field name are both ran through this function.
        /// You should do things like trimming, removing whitespace, removing underscores,
        /// and making casing changes to ignore case.
        /// </summary>
        public virtual Func<string, string> PrepareHeaderForMatch { get; set; } = header => header;

        /// <summary>
        /// Gets or sets a value indicating whether references
        /// should be ignored when auto mapping. True to ignore
        /// references, otherwise false. Default is false.
        /// </summary>
        public virtual bool IgnoreReferences { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fields
        /// should be trimmed. True to trim fields,
        /// otherwise false. Default is false.
        /// </summary>
        public virtual bool TrimFields { get; set; }

        /// <summary>
        /// Gets or sets the delimiter used to separate fields.
        /// Default is ',';
        /// </summary>
        public virtual string Delimiter
        {
            get => _delimiter;

            set
            {
                switch (value)
                {
                    case "\n":
                        throw new CsvConfigurationException("Newline is not a valid delimiter.");

                    case "\r":
                        throw new CsvConfigurationException("Carriage return is not a valid delimiter.");

                    case "\0":
                        throw new CsvConfigurationException("Null is not a valid delimiter.");
                }

                if (value == Convert.ToString(_quote))
                {
                    throw new CsvConfigurationException("You can not use the quote as a delimiter.");
                }

                _delimiter = value;

                BuildRequiredQuoteChars();
            }
        }

        /// <summary>
        /// Gets or sets the character used to quote fields.
        /// Default is '"'.
        /// </summary>
        public virtual char Quote
        {
            get => _quote;
            set
            {
                if (value == '\n')
                {
                    throw new CsvConfigurationException("Newline is not a valid quote.");
                }

                if (value == '\r')
                {
                    throw new CsvConfigurationException("Carriage return is not a valid quote.");
                }

                if (value == '\0')
                {
                    throw new CsvConfigurationException("Null is not a valid quote.");
                }

                if (Convert.ToString(value) == _delimiter)
                {
                    throw new CsvConfigurationException("You can not use the delimiter as a quote.");
                }

                _quote = value;

                _quoteString = Convert.ToString(value, _cultureInfo);
                _doubleQuoteString = _quoteString + _quoteString;
            }
        }

        /// <summary>
        /// Gets a string representation of the currently configured Quote character.
        /// </summary>
        /// <value>
        /// The new quote string.
        /// </value>
        public virtual string QuoteString => _quoteString;

        /// <summary>
        /// Gets a string representation of two of the currently configured Quote characters.
        /// </summary>
        /// <value>
        /// The new double quote string.
        /// </value>
        public virtual string DoubleQuoteString => _doubleQuoteString;

        /// <summary>
        /// Gets an array characters that require
        /// the field to be quoted.
        /// </summary>
        public virtual char[] QuoteRequiredChars => _quoteRequiredChars;

        /// <summary>
        /// Gets or sets the character used to denote
        /// a line that is commented out. Default is '#'.
        /// </summary>
        public virtual char Comment { get; set; } = '#';

        /// <summary>
        /// Gets or sets a value indicating if comments are allowed.
        /// True to allow commented out lines, otherwise false.
        /// </summary>
        public virtual bool AllowComments { get; set; }

        /// <summary>
        /// Gets or sets the size of the buffer
        /// used for reading and writing CSV files.
        /// Default is 2048.
        /// </summary>
        public virtual int BufferSize { get; set; } = 2048;

        /// <summary>
        /// Gets or sets a value indicating whether all fields are quoted when writing,
        /// or just ones that have to be. <see cref="QuoteAllFields" /> and
        /// <see cref="QuoteNoFields" /> cannot be true at the same time. Turning one
        /// on will turn the other off.
        /// </summary>
        /// <value>
        /// <c>true</c> if all fields should be quoted; otherwise, <c>false</c>.
        /// </value>
        public virtual bool QuoteAllFields
        {
            get => _quoteAllFields;
            set
            {
                _quoteAllFields = value;
                if (_quoteAllFields && _quoteNoFields)
                {
                    // Both can't be true at the same time.
                    _quoteNoFields = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether no fields are quoted when writing.
        /// <see cref="QuoteAllFields" /> and <see cref="QuoteNoFields" /> cannot be true
        /// at the same time. Turning one on will turn the other off.
        /// </summary>
        /// <value>
        /// <c>true</c> if [quote no fields]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool QuoteNoFields
        {
            get => _quoteNoFields;
            set
            {
                _quoteNoFields = value;
                if (_quoteNoFields && _quoteAllFields)
                {
                    // Both can't be true at the same time.
                    _quoteAllFields = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the number of bytes should
        /// be counted while parsing. Default is false. This will slow down parsing
        /// because it needs to get the byte count of every char for the given encoding.
        /// The <see cref="Encoding" /> needs to be set correctly for this to be accurate.
        /// </summary>
        public virtual bool CountBytes { get; set; }

        /// <summary>
        /// Gets or sets the encoding used when counting bytes.
        /// </summary>
        public virtual Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets or sets the culture info used to read an write CSV files.
        /// </summary>
        public virtual CultureInfo CultureInfo
        {
            get => _cultureInfo;
            set => _cultureInfo = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether empty rows should be skipped when reading.
        /// A record is considered empty if all fields are empty.
        /// </summary>
        /// <value>
        /// <c>true</c> if [skip empty rows]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SkipEmptyRecords { get; set; }

        /// <summary>
        /// Gets or sets the callback that will be called to
        /// determine whether to skip the given record or not.
        /// This overrides the <see cref="SkipEmptyRecords" /> setting.
        /// </summary>
        public virtual Func<string[], bool> ShouldSkipRecord { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if quotes should be
        /// ignored when parsing and treated like any other character.
        /// </summary>
        public virtual bool IgnoreQuotes { get; set; }

        /// <summary>
        /// Unescape single and double quotes in string fields.
        /// Defaults to [true].
        /// </summary>
        public bool UnescapeQuotes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if private
        /// properties/fields should be read from and written to.
        /// True to include private properties/fields, otherwise false. Default is false.
        /// </summary>
        public virtual bool IncludePrivateMembers { get; set; }

        /// <summary>
        /// Gets or sets the member types that are used when auto mapping.
        /// MemberTypes are flags, so you can choose more than one.
        /// Default is Properties.
        /// </summary>
        public virtual MemberTypes MemberTypes { get; set; } = MemberTypes.Properties;

        /// <summary>
        /// Gets or sets a value indicating if blank lines
        /// should be ignored when reading.
        /// True to ignore, otherwise false. Default is true.
        /// </summary>
        public virtual bool IgnoreBlankLines { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating if an Excel specific
        /// format should be used when writing fields containing
        /// numeric values. e.g. 00001 -> ="00001"
        /// </summary>
        public virtual bool UseExcelLeadingZerosFormatForNumerics { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if headers of reference
        /// properties/fields should get prefixed by the parent property/field
        /// name when automapping.
        /// True to prefix, otherwise false. Default is false.
        /// </summary>
        public virtual bool PrefixReferenceHeaders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if an exception should
        /// be thrown when bad field data is detected.
        /// True to throw, otherwise false. Default is false.
        /// </summary>
        public virtual bool ThrowOnBadData { get; set; }

        /// <summary>
        /// Gets or sets a method that gets called when bad
        /// data is detected.
        /// </summary>
        public virtual Action<string> BadDataCallback { get; set; }

        /// <summary>
        /// Creates a new CsvConfiguration.
        /// </summary>
        public CsvConfiguration()
        {
            BuildRequiredQuoteChars();
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// exceptions that occur during reading
        /// should be ignored. True to ignore exceptions,
        /// otherwise false. Default is false.
        /// </summary>
        public virtual bool IgnoreReadingExceptions { get; set; }

        /// <summary>
        /// Gets or sets the callback that is called when a reading
        /// exception occurs. This will only happen when
        /// <see cref="IgnoreReadingExceptions" /> is true.
        /// </summary>
        public virtual Action<CsvHelperException, ICsvReader> ReadingExceptionCallback { get; set; }

        /// <summary>
        /// Convert empty or literal null ("NULL", "null") values to null.
        /// </summary>
        public bool TranslateLiteralNulls { get; set; }

        /// <summary>
        /// The configured <see cref="CsvClassMap" />s.
        /// </summary>
        public virtual CsvClassMapCollection Maps => _maps;

        /// <summary>
        /// Gets or sets a value indicating that during writing if a new
        /// object should be created when a reference property/field is null.
        /// True to create a new object and use it's defaults for the
        /// fields, or false to leave the fields empty for all the
        /// reference property/field's properties/fields.
        /// </summary>
        public virtual bool UseNewObjectForNullReferenceMembers { get; set; } = true;

        /// <summary>
        /// Use a <see cref="CsvClassMap{T}" /> to configure mappings.
        /// When using a class map, no properties/fields are mapped by default.
        /// Only properties/fields specified in the mapping are used.
        /// </summary>
        /// <typeparam name="TMap">The type of mapping class to use.</typeparam>
        public virtual TMap RegisterClassMap<TMap>() where TMap : CsvClassMap
        {
            var map = ReflectionHelper.CreateInstance<TMap>();
            RegisterClassMap(map);

            return map;
        }

        /// <summary>
        /// Use a <see cref="CsvClassMap{T}" /> to configure mappings.
        /// When using a class map, no properties/fields are mapped by default.
        /// Only properties/fields specified in the mapping are used.
        /// </summary>
        /// <param name="classMapType">The type of mapping class to use.</param>
        public virtual CsvClassMap RegisterClassMap(Type classMapType)
        {
            if (!typeof(CsvClassMap).GetTypeInfo().IsAssignableFrom(classMapType))
            {
                throw new ArgumentException("The class map type must inherit from CsvClassMap.");
            }

            var map = (CsvClassMap)ReflectionHelper.CreateInstance(classMapType);
            RegisterClassMap(map);

            return map;
        }

        /// <summary>
        /// Registers the class map.
        /// </summary>
        /// <param name="map">The class map to register.</param>
        public virtual void RegisterClassMap(CsvClassMap map)
        {
            if (map.Constructor == null && map.PropertyMaps.Count == 0 && map.ReferenceMaps.Count == 0)
            {
                throw new CsvConfigurationException("No mappings were specified in the CsvClassMap.");
            }

            Maps.Add(map);
        }

        /// <summary>
        /// Creates a map of the specified class annotated with <see cref="CsvFieldAttribute" /> and registers it.
        /// </summary>
        /// <typeparam name="T">Type of the class to be registered.</typeparam>
        /// <returns></returns>
        public virtual CsvClassMap RegisterClass<T>() where T : class
        {
            var map = Map<T>();

            RegisterClassMap(map);

            return map;
        }

        /// <summary>
        /// Unregisters the class map.
        /// </summary>
        /// <typeparam name="TMap">The map type to unregister.</typeparam>
        public virtual void UnregisterClassMap<TMap>()
            where TMap : CsvClassMap
        {
            UnregisterClassMap(typeof(TMap));
        }

        /// <summary>
        /// Unregisters the class map.
        /// </summary>
        /// <param name="classMapType">The map type to unregister.</param>
        public virtual void UnregisterClassMap(Type classMapType)
        {
            _maps.Remove(classMapType);
        }

        /// <summary>
        /// Unregisters all class maps.
        /// </summary>
        public virtual void UnregisterClassMap()
        {
            _maps.Clear();
        }

        /// <summary>
        /// Generates a <see cref="CsvClassMap" /> for the type.
        /// </summary>
        /// <typeparam name="T">The type to generate the map for.</typeparam>
        /// <returns>The generate map.</returns>
        public virtual CsvClassMap AutoMap<T>()
        {
            return AutoMap(typeof(T));
        }

        /// <summary>
        /// Generates a <see cref="CsvClassMap" /> for the type.
        /// </summary>
        /// <param name="type">The type to generate for the map.</param>
        /// <returns>The generate map.</returns>
        public virtual CsvClassMap AutoMap(Type type)
        {
            var mapType = typeof(DefaultCsvClassMap<>).MakeGenericType(type);
            var map = (CsvClassMap)ReflectionHelper.CreateInstance(mapType);
            map.AutoMap(new AutoMapOptions(this));

            return map;
        }

        /// <summary>
        /// Creates a map of the specified class annotated with <see cref="CsvFieldAttribute" />.
        /// </summary>
        /// <typeparam name="T">Type of the class to be registered.</typeparam>
        /// <returns></returns>
        public virtual CsvClassMap Map<T>()
        {
            var map = new DefaultCsvClassMap<T>();

            foreach (var propertyInfo in typeof(T).GetTypeInfo().GetProperties())
            {
                var fieldInfo = propertyInfo.GetCustomAttribute<CsvFieldAttribute>();

                if (fieldInfo == null)
                {
                    continue;
                }

                var propertyMap = new CsvPropertyMap(propertyInfo);

                if (fieldInfo.Name != null)
                {
                    propertyMap.Name(fieldInfo.Name);
                }

                if (fieldInfo.Index.HasValue)
                {
                    propertyMap.NameIndex(fieldInfo.Index.Value);
                }

                if (fieldInfo.TypeConverter != null)
                {
                    propertyMap.TypeConverter(fieldInfo.TypeConverter);
                }

                propertyMap.Data.FieldPreprocessorSettings = fieldInfo;

                map.PropertyMaps.Add(propertyMap);
            }

            return map;
        }

        /// <summary>
        /// Builds the values for the RequiredQuoteChars property.
        /// </summary>
        private void BuildRequiredQuoteChars()
        {
            _quoteRequiredChars = _delimiter.Length > 1
                ? new[] { '\r', '\n' }
                : new[] { '\r', '\n', _delimiter[0] };
        }
    }
}