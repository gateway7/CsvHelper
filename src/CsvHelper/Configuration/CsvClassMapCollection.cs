﻿// Copyright 2009-2015 Josh Close and Contributors
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// http://csvhelper.com

namespace CsvHelper.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Collection that holds CsvClassMaps for record types.
    /// </summary>
    public class CsvClassMapCollection
    {
        private readonly Dictionary<Type, CsvClassMap> _data = new Dictionary<Type, CsvClassMap>();

        /// <summary>
        /// Gets the <see cref="CsvClassMap" /> for the specified record type.
        /// </summary>
        /// <value>
        /// The <see cref="CsvClassMap" />.
        /// </value>
        /// <param name="type">The record type.</param>
        /// <returns>The <see cref="CsvClassMap" /> for the specified record type.</returns>
        public virtual CsvClassMap this[Type type]
        {
            get
            {
                // Go up the inheritance tree to find the matching type.
                // We can't use IsAssignableFrom because both a child
                // and it's parent/grandparent/etc could be mapped.
                var currentType = type;
                while (true)
                {
                    if (currentType == type)
                    {
                        return _data.ContainsKey(currentType) ? _data[currentType] : null;
                    }

                    currentType = type.GetTypeInfo().BaseType;
                    if (currentType == null)
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Finds the <see cref="CsvClassMap" /> for the specified record type.
        /// </summary>
        /// <typeparam name="T">The record type.</typeparam>
        /// <returns>The <see cref="CsvClassMap" /> for the specified record type.</returns>
        public virtual CsvClassMap<T> Find<T>()
        {
            return (CsvClassMap<T>)this[typeof(T)];
        }

        /// <summary>
        /// Adds the specified map for it's record type. If a map
        /// already exists for the record type, the specified
        /// map will replace it.
        /// </summary>
        /// <param name="map">The map.</param>
        public virtual void Add(CsvClassMap map)
        {
            var type = GetGenericCsvClassMapType(map.GetType()).GetTypeInfo().GetGenericArguments().First();

            if (_data.ContainsKey(type))
            {
                _data[type] = map;
            }
            else
            {
                _data.Add(type, map);
            }
        }

        /// <summary>
        /// Removes the class map.
        /// </summary>
        /// <param name="classMapType">The class map type.</param>
        internal virtual void Remove(Type classMapType)
        {
            if (!typeof(CsvClassMap).GetTypeInfo().IsAssignableFrom(classMapType))
            {
                throw new ArgumentException("The class map type must inherit from CsvClassMap.");
            }

            var type = GetGenericCsvClassMapType(classMapType).GetTypeInfo().GetGenericArguments().First();

            _data.Remove(type);
        }

        /// <summary>
        /// Removes all maps.
        /// </summary>
        internal virtual void Clear()
        {
            _data.Clear();
        }

        /// <summary>
        /// Goes up the inheritance tree to find the type instance of CsvClassMap{}.
        /// </summary>
        /// <param name="type">The type to traverse.</param>
        /// <returns>The type that is CsvClassMap{}.</returns>
        private Type GetGenericCsvClassMapType(Type type)
        {
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(CsvClassMap<>))
            {
                return type;
            }

            return GetGenericCsvClassMapType(type.GetTypeInfo().BaseType);
        }
    }
}
