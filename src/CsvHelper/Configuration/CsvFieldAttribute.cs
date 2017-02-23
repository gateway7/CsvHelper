namespace CsvHelper.Configuration
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CsvFieldAttribute : Attribute, IFieldPreprocessingSettings
    {
        /// <summary>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        public int? Index { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        public CsvFieldAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        public CsvFieldAttribute(int index)
        {
            Index = index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="regexMatchPattern"></param>
        /// <param name="regexReplacementPattern"></param>
        public CsvFieldAttribute(string name, string regexMatchPattern, string regexReplacementPattern) : this(name)
        {
            SetRegexReplacementInfo(regexMatchPattern, regexReplacementPattern);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="regexMatchPattern"></param>
        /// <param name="regexReplacementPattern"></param>
        public CsvFieldAttribute(int index, string regexMatchPattern, string regexReplacementPattern) : this(index)
        {
            SetRegexReplacementInfo(regexMatchPattern, regexReplacementPattern);
        }

        /// <summary>
        /// A mandatory prefix expected to appear in the field. Absence of such will result in an exception.
        /// </summary>
        public string ExpectedPrefix { get; set; }

        /// <summary>
        /// A mandatory suffix expected to appear in the field. Absence of such will result in an exception.
        /// </summary>
        public string ExpectedSuffix { get; set; }

        /// <summary>
        /// Instructs the parser to trim <see cref="ExpectedPrefix" />, if such was specified.
        /// </summary>
        public bool TrimPrefix { get; set; }

        /// <summary>
        /// Instructs the parser to trim <see cref="ExpectedSuffix" />, if such was specified.
        /// </summary>
        public bool TrimSuffix { get; set; }

        /// <summary>
        /// The amount of characters to trim from the beginning of the firld. Cannot be specified together with <see cref="TrimPrefix" />.
        /// </summary>
        public int TrimStart { get; set; }

        /// <summary>
        /// The amount of characters to trim from the end of the field. Cannot be specified together with <see cref="TrimSuffix" />.
        /// </summary>
        public int TrimEnd { get; set; }

        /// <summary>
        /// The [pattern] (second) argument in <see cref="Regex.Replace(string, string, string)"/>
        /// </summary>
        public string RegexMatchPattern { get; private set; }

        /// <summary>
        /// The [replacement] (third) argument in <see cref="Regex.Replace(string, string, string)"/>
        /// </summary>
        public string RegexReplacementPattern { get; private set; }

        private void SetRegexReplacementInfo(string regexMatchPattern, string regexReplacementPattern)
        {
            if (string.IsNullOrWhiteSpace(regexMatchPattern))
            {
                throw new ArgumentNullException(nameof(regexMatchPattern));
            }

            if (string.IsNullOrWhiteSpace(regexReplacementPattern))
            {
                throw new ArgumentNullException(nameof(regexReplacementPattern));
            }

            RegexMatchPattern = regexMatchPattern;
            RegexReplacementPattern = regexReplacementPattern;
        }
    }
}