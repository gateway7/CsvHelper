namespace CsvHelper.Configuration
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CsvFieldAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Index { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public CsvFieldAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public CsvFieldAttribute(int index)
        {
            Index = index;
        }
    }
}