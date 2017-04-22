// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.TypeConversion
{
    using System;
    using System.Reflection;
    using Configuration;

    /// <summary>
    /// Converts an <see cref="object" /> to and from a <see cref="string" />.
    /// </summary>
    public class DefaultTypeConverter : ITypeConverter
    {
        /// <summary>
        /// Converts the object to a string.
        /// </summary>
        /// <param name="value">The object to convert to a string.</param>
        /// <param name="row">The <see cref="ICsvWriterRow" /> for the current record.</param>
        /// <param name="propertyMapData">The <see cref="CsvPropertyMapData" /> for the property/field being written.</param>
        /// <returns>The string representation of the object.</returns>
        public virtual string ConvertToString(object value, ICsvWriterRow row, CsvPropertyMapData propertyMapData)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is IFormattable formattable)
            {
                return formattable.ToString(propertyMapData.TypeConverterOptions.Format, propertyMapData.TypeConverterOptions.CultureInfo);
            }

            return value.ToString();
        }

        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="text">The string to convert to the specified type.</param>
        /// <param name="row">The <see cref="ICsvReaderRow" /> for the current record.</param>
        /// <param name="propertyMapData">The <see cref="CsvPropertyMapData" /> for the property/field being created.</param>
        /// <returns>The object created from the string.</returns>
        public virtual T ConvertFromString<T>(string text, ICsvReaderRow row, CsvPropertyMapData propertyMapData)
        {
            return (T)ConvertFromString(typeof(T), text, row, propertyMapData);

            //var converterOptions = row?.Configuration.TypeConverterOptions?.Get<T>() ?? propertyMapData.TypeConverterOptions;

            //if (converterOptions.TreatNullAsDefault && string.IsNullOrWhiteSpace(text))
            //{
            //    return default(T); 
            //}

            //throw new CsvTypeConverterException("The conversion cannot be performed.");
        }

        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="text">The string to convert to the specified type.</param>
        /// <param name="row">The <see cref="ICsvReaderRow" /> for the current record.</param>
        /// <param name="propertyMapData">The <see cref="CsvPropertyMapData" /> for the property/field being created.</param>
        /// <returns>The object created from the string.</returns>
        public virtual object ConvertFromString(Type type, string text, ICsvReaderRow row, CsvPropertyMapData propertyMapData)
        {
            var converterOptions = row?.Configuration.TypeConverterOptions?[type] ?? propertyMapData.TypeConverterOptions;

            if (converterOptions.TreatNullAsDefault && string.IsNullOrWhiteSpace(text))
            {
                return GetDefault(type); 
            }

            throw new CsvTypeConverterException("The conversion cannot be performed.");
        }

        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="text">The string to convert to an object.</param>
        /// <param name="row">The <see cref="ICsvReaderRow" /> for the current record.</param>
        /// <param name="propertyMapData">The <see cref="CsvPropertyMapData" /> for the property/field being created.</param>
        /// <returns>The object created from the string.</returns>
        public virtual object ConvertFromString(string text, ICsvReaderRow row, CsvPropertyMapData propertyMapData)
        {
            throw new CsvTypeConverterException("The conversion cannot be performed.");
        }

        private static object GetDefault(Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}