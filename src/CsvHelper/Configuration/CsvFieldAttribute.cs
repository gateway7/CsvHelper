namespace CsvHelper.Configuration
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CsvFieldAttribute : Attribute, IFieldPreprocessorSettings
    {
        /// <summary>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        public int? Index { get; set; }

        /// <summary>
        /// </summary>
        public Type TypeConverter { get; set; }

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
        /// </summary>
        /// <param name="name"></param>
        /// <param name="regexMatchPattern"></param>
        /// <param name="regexReplacementPattern"></param>
        public CsvFieldAttribute(string name, string regexMatchPattern, string regexReplacementPattern) : this(name)
        {
            SetRegexReplacementInfo(regexMatchPattern, regexReplacementPattern);
        }

        /// <summary>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="regexMatchPattern"></param>
        /// <param name="regexReplacementPattern"></param>
        public CsvFieldAttribute(int index, string regexMatchPattern, string regexReplacementPattern) : this(index)
        {
            SetRegexReplacementInfo(regexMatchPattern, regexReplacementPattern);
        }

        /// <summary>
        /// An optional prefix expected to appear in the field. Set <see cref="IsPrefixMandatory" /> to true, is the prefix is
        /// mandatory.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// If true, the value of <see cref="Prefix" /> must be defined. Absence
        /// of the latter in the field value will result in an exception.
        /// Defaults to false.
        /// </summary>
        public bool IsPrefixMandatory { get; set; }

        /// <summary>
        /// If true, trims the prefix value defined in <see cref="Prefix" />.
        /// </summary>
        public bool TrimPrefix { get; set; }

        /// <summary>
        /// An optional suffix expected to appear in the field. Set <see cref="IsPrefixMandatory" /> to true, is the prefix is
        /// mandatory.
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// If true, the value of <see cref="Suffix" /> must be defined. Absence
        /// of the latter in the field value will result in an exception.
        /// Defaults to false.
        /// </summary>
        public bool IsSuffixMandatory { get; set; }

        /// <summary>
        /// If true, trims the suffix value defined in <see cref="Suffix" />.
        /// </summary>
        public bool TrimSuffix { get; set; }

        /// <summary>
        /// The amount of characters to trim from the beginning of the field. Cannot be specified together with
        /// <see cref="TrimPrefix" />.
        /// </summary>
        public int TrimStart { get; set; }

        /// <summary>
        /// The amount of characters to trim from the end of the field. Cannot be specified together with <see cref="TrimSuffix" />
        /// .
        /// </summary>
        public int TrimEnd { get; set; }

        /// <summary>
        /// The [pattern] (second) argument in <see cref="Regex.Replace(string, string, string)" />
        /// </summary>
        public string RegexMatchPattern { get; private set; }

        /// <summary>
        /// The [replacement] (third) argument in <see cref="Regex.Replace(string, string, string)" />
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