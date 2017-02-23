namespace CsvHelper.Configuration
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides information on preprocessing to be performed on the CSV field after extraction.
    /// </summary>
    public interface IFieldPreprocessorSettings
    {
        /// <summary>
        /// A mandatory prefix expected to appear in the field. Absence of such will result in an exception.
        /// </summary>
        string ExpectedPrefix { get; set; }

        /// <summary>
        /// A mandatory suffix expected to appear in the field. Absence of such will result in an exception.
        /// </summary>
        string ExpectedSuffix { get; set; }

        /// <summary>
        /// Instructs the parser to trim <see cref="ExpectedPrefix" />, if such was specified.
        /// </summary>
        bool TrimPrefix { get; set; }

        /// <summary>
        /// Instructs the parser to trim <see cref="ExpectedSuffix" />, if such was specified.
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