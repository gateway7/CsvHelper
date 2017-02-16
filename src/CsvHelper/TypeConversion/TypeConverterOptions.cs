// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.TypeConversion
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Options used when doing type conversion.
    /// </summary>
    public class TypeConverterOptions
    {
        private static readonly string[] DefaultBooleanTrueValues = { "yes", "y" };

        private static readonly string[] DefaultBooleanFalseValues = { "no", "n" };

        private static readonly string[] DefaultNullValues = { "null", "NULL" };

        /// <summary>
        /// Gets or sets the culture info.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Gets or sets the date time style.
        /// </summary>
        public DateTimeStyles? DateTimeStyle { get; set; }

        /// <summary>
        /// Gets or sets the time span style.
        /// </summary>
        public TimeSpanStyles? TimeSpanStyle { get; set; }

        /// <summary>
        /// Gets or sets the number style.
        /// </summary>
        public NumberStyles? NumberStyle { get; set; }

        /// <summary>
        /// Gets or sets the string format.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Treat null string values as default for the field's type. 
        /// </summary>
        public bool TreatNullAsDefault { get; set; }

        /// <summary>
        /// Gets the list of values that can be
        /// used to represent a boolean of true.
        /// </summary>
        public List<string> BooleanTrueValues { get; } = new List<string>(DefaultBooleanTrueValues);

        /// <summary>
        /// Gets the list of values that can be
        /// used to represent a boolean of false.
        /// </summary>
        public List<string> BooleanFalseValues { get; } = new List<string>(DefaultBooleanFalseValues);

        /// <summary>
        /// Gets the list of values that can be used to represent a null value.
        /// </summary>
        public List<string> NullValues { get; } = new List<string>(DefaultNullValues);

        /// <summary>
        /// Returns true if the specified value is considered null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsNullValue(string value) => NullValues.Any(nullValue => value == nullValue);

        /// <summary>
        /// Merges TypeConverterOptions by applying the values of sources in order to a
        /// new TypeConverterOptions instance.
        /// </summary>
        /// <param name="sources">The sources that will be applied.</param>
        /// <returns>A new instance of TypeConverterOptions with the source applied to it.</returns>
        public static TypeConverterOptions Merge(params TypeConverterOptions[] sources)
        {
            var options = new TypeConverterOptions();

            foreach (var source in sources)
            {
                if (source == null)
                {
                    continue;
                }

                if (source.CultureInfo != null)
                {
                    options.CultureInfo = source.CultureInfo;
                }

                if (source.DateTimeStyle != null)
                {
                    options.DateTimeStyle = source.DateTimeStyle;
                }

                if (source.TimeSpanStyle != null)
                {
                    options.TimeSpanStyle = source.TimeSpanStyle;
                }

                if (source.NumberStyle != null)
                {
                    options.NumberStyle = source.NumberStyle;
                }

                if (source.Format != null)
                {
                    options.Format = source.Format;
                }

                // Only change the values if they are different than the defaults.
                // This means there were explicit changes made to the options.

                if (!DefaultBooleanTrueValues.SequenceEqual(source.BooleanTrueValues))
                {
                    options.BooleanTrueValues.Clear();
                    options.BooleanTrueValues.AddRange(source.BooleanTrueValues);
                }

                if (!DefaultBooleanFalseValues.SequenceEqual(source.BooleanFalseValues))
                {
                    options.BooleanFalseValues.Clear();
                    options.BooleanFalseValues.AddRange(source.BooleanFalseValues);
                }

                if (!DefaultNullValues.SequenceEqual(source.NullValues))
                {
                    options.NullValues.Clear();
                    options.NullValues.AddRange(source.NullValues);
                }
            }

            return options;
        }
    }
}