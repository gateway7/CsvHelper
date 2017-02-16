// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.TypeConversion
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Creates <see cref="TypeConverterOptions" />.
    /// </summary>
    public class TypeConverterOptionsCollection
    {
        private readonly Dictionary<Type, TypeConverterOptions> _options = new Dictionary<Type, TypeConverterOptions>();

        /// <summary>
        /// Returns global options for all types.
        /// </summary>
        public TypeConverterOptions Default { get; } = new TypeConverterOptions();

        /// <summary>
        /// Adds the <see cref="TypeConverterOptions" /> for the given <see cref="Type" />.
        /// </summary>
        /// <param name="type">The type the options are for.</param>
        /// <param name="options">The options.</param>
        public void Add(Type type, TypeConverterOptions options)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options[type] = options;
        }

        /// <summary>
        /// Adds the <see cref="TypeConverterOptions" /> for the given <see cref="Type" />.
        /// </summary>
        /// <typeparam name="T">The type the options are for.</typeparam>
        /// <param name="options">The options.</param>
        public void Add<T>(TypeConverterOptions options)
        {
            Add(typeof(T), options);
        }

        /// <summary>
        /// Removes the <see cref="TypeConverterOptions" /> for the given type.
        /// </summary>
        /// <param name="type">The type to remove the options for.</param>
        public void Remove(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _options.Remove(type);
        }

        /// <summary>
        /// Removes the <see cref="TypeConverterOptions" /> for the given type.
        /// </summary>
        /// <typeparam name="T">The type to remove the options for.</typeparam>
        public void Remove<T>()
        {
            Remove(typeof(T));
        }

        /// <summary>
        /// Removes all options from the collection.
        /// </summary>
        public void Clear()
        {
            _options.Clear();
        }

        /// <summary>
        /// Returns <see cref="TypeConverterOptions"/> for the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TypeConverterOptions this[Type type] => Get(type);

        /// <summary>
        /// Gets the <see cref="TypeConverterOptions" /> for the given <see cref="Type" />. If none exists, spawns a new one from <see cref="Default"/>.
        /// </summary>
        /// <param name="type">The type the options are for.</param>
        /// <returns>The options for the given type.</returns>
        public TypeConverterOptions Get(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException();
            }

            _options.TryGetValue(type, out TypeConverterOptions options);

            if (options == null)
            {
                options = TypeConverterOptions.Merge(Default);
                _options.Add(type, options);
            }

            return options;
        }

        /// <summary>
        /// Get the <see cref="TypeConverterOptions" /> for the given <see cref="Type" />. If none exists, creates a new one.
        /// </summary>
        /// <typeparam name="T">The type the options are for.</typeparam>
        /// <returns>The options for the given type.</returns>
        public TypeConverterOptions Get<T>()
        {
            return Get(typeof(T));
        }
    }
}