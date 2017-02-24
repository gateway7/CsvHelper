namespace CsvHelper.Configuration
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides information on preprocessing to be performed on the CSV field after extraction.
    /// </summary>
    public interface IFieldPreprocessorSettings
    {
        /// <summary>
        /// An optional prefix expected to appear in the field. Set <see cref="IsPrefixMandatory"/> to true, is the prefix is mandatory.
        /// </summary>
        string Prefix { get; set; }

        /// <summary>
        /// If true, the value of <see cref="Prefix"/> must be defined. Absence 
        /// of the latter in the field value will result in an exception.
        /// Defaults to false.
        /// </summary>
        bool IsPrefixMandatory { get; set; }

        /// <summary>
        /// If true, trims the prefix value defined in <see cref="Prefix" />.
        /// </summary>
        bool TrimPrefix { get; set; }

        /// <summary>
        /// An optional suffix expected to appear in the field. Set <see cref="IsPrefixMandatory"/> to true, is the prefix is mandatory.
        /// </summary>
        string Suffix { get; set; }

        /// <summary>
        /// If true, the value of <see cref="Suffix"/> must be defined. Absence 
        /// of the latter in the field value will result in an exception.
        /// Defaults to false.
        /// </summary>
        bool IsSuffixMandatory { get; set; }

        /// <summary>
        /// If true, trims the suffix value defined in <see cref="Suffix" />.
        /// </summary>
        bool TrimSuffix { get; set; }

        /// <summary>
        /// The amount of characters to trim from the beginning of the firld. Cannot be specified together with <see cref="TrimPrefix"/>.
        /// </summary>
        int TrimStart { get; set; }

        /// <summary>
        /// The amount of characters to trim from the end of the field. Cannot be specified together with <see cref="TrimSuffix"/>.
        /// </summary>
        int TrimEnd { get; set; }

        /// <summary>
        /// Return the [pattern] (second) argument in <see cref="Regex.Replace(string, string, string)"/>.
        /// Requires <see cref="RegexReplacementPattern"/> and cannot be used in conjunction with other, non-regex settings.
        /// </summary>
        string RegexMatchPattern { get; }

        /// <summary>
        /// Returns the [replacement] (third) argument in <see cref="Regex.Replace(string, string, string)"/>
        /// Requires <see cref="RegexMatchPattern"/> and cannot be used in conjunction with other, non-regex settings.
        /// </summary>
        string RegexReplacementPattern { get; }
    }
}