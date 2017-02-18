namespace CsvHelper
{
    using System;

    /// <summary>
    /// Builds CSV records.
    /// </summary>
    public class RecordBuilder
    {
        private const int DEFAULT_CAPACITY = 16;

        private string[] _record;

        /// <summary>
        /// The number of records.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The total record capacity.
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// Creates a new <see cref="RecordBuilder" /> using defaults.
        /// </summary>
        public RecordBuilder() : this(DEFAULT_CAPACITY) {}

        /// <summary>
        /// Creates a new <see cref="RecordBuilder" /> using the given capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        public RecordBuilder(int capacity)
        {
            Capacity = capacity > 0 ? capacity : DEFAULT_CAPACITY;

            _record = new string[capacity];
        }

        /// <summary>
        /// Adds a new field to the <see cref="RecordBuilder" />.
        /// </summary>
        /// <param name="field">The field to add.</param>
        /// <returns>The current instance of the <see cref="RecordBuilder" />.</returns>
        public virtual RecordBuilder Add(string field)
        {
            if (Length == _record.Length)
            {
                Capacity = Capacity * 2;
                Array.Resize(ref _record, Capacity);
            }

            _record[Length] = field;
            Length++;

            return this;
        }

        /// <summary>
        /// Clears the records.
        /// </summary>
        /// <returns>The current instance of the <see cref="RecordBuilder" />.</returns>
        public virtual RecordBuilder Clear()
        {
            Length = 0;

            return this;
        }

        /// <summary>
        /// Returns the record as an <see cref="T:string[]" />.
        /// </summary>
        /// <returns>The record as an <see cref="T:string[]" />.</returns>
        public virtual string[] ToArray()
        {
            var array = new string[Length];
            Array.Copy(_record, array, Length);

            return array;
        }
    }
}