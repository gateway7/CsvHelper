namespace CsvHelper
{
    using System;
    using System.Text.RegularExpressions;
    using Configuration;

    internal static class FieldPreprocessor
    {
        public static string ProcessField(string value, ICsvReaderConfiguration readerConfiguration, IFieldPreprocessorSettings fieldPreprocessorSettings = null)
        {
            if (readerConfiguration.TranslateLiteralNulls && (string.IsNullOrWhiteSpace(value) || value.Equals("null", StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (fieldPreprocessorSettings == null)
            {
                return value;
            }

            if (fieldPreprocessorSettings.RegexMatchPattern != null)
            {
                if (fieldPreprocessorSettings.Prefix != null || fieldPreprocessorSettings.Suffix != null || fieldPreprocessorSettings.TrimStart > 0 || fieldPreprocessorSettings.TrimEnd > 0)
                {
                    throw new CsvConfigurationException("Regex replacement cannot be used in conjunction with other preprocessing methods.");
                }

                value = Regex.Replace(value, fieldPreprocessorSettings.RegexMatchPattern, fieldPreprocessorSettings.RegexReplacementPattern);

                return value;
            }

            if (fieldPreprocessorSettings.Prefix != null)
            {
                bool HasPrefix() => value.StartsWith(fieldPreprocessorSettings.Prefix);

                bool? hasPrefix = null;

                if (fieldPreprocessorSettings.IsPrefixMandatory)
                {
                    hasPrefix = HasPrefix();

                    if (!hasPrefix.GetValueOrDefault(false))
                    {
                        throw new CsvBadDataException($"Expected field prefix [{fieldPreprocessorSettings.Prefix}] in [{value}].");
                    }
                }

                if (fieldPreprocessorSettings.TrimPrefix && (hasPrefix.GetValueOrDefault(false) || HasPrefix()))
                {
                    value = value.Remove(0, fieldPreprocessorSettings.Prefix.Length);
                }
            }

            if (fieldPreprocessorSettings.Suffix != null)
            {
                bool HasSuffix() => value.EndsWith(fieldPreprocessorSettings.Suffix);

                bool? hasSuffix = null;

                if (fieldPreprocessorSettings.IsSuffixMandatory)
                {
                    hasSuffix = HasSuffix();

                    if (!hasSuffix.GetValueOrDefault(false))
                    {
                        throw new CsvBadDataException($"Expected field suffix [{fieldPreprocessorSettings.Suffix}] in [{value}].");
                    }
                }

                if (fieldPreprocessorSettings.TrimSuffix && (hasSuffix.GetValueOrDefault(false) || HasSuffix()))
                {
                    value = value.Substring(0, value.Length - fieldPreprocessorSettings.Suffix.Length);
                }
            }

            if (fieldPreprocessorSettings.TrimStart > 0)
            {
                if (fieldPreprocessorSettings.TrimPrefix)
                {
                    throw new CsvConfigurationException("Field preprocessing settings cannot have both TrimPrefix and TrimStart flags set.");
                }

                if (fieldPreprocessorSettings.TrimStart >= value.Length)
                {
                    throw new CsvConfigurationException($"Trimming the field [{value}] would result in its total elimination. Check specific IFieldPreprocessingSettings.TrimStart settings.");
                }

                value = value.Remove(0, fieldPreprocessorSettings.TrimStart);
            }

            if (fieldPreprocessorSettings.TrimEnd > 0)
            {
                if (fieldPreprocessorSettings.TrimSuffix)
                {
                    throw new CsvConfigurationException("Field preprocessing settings cannot have both TrimSuffix and TrimEnd flags set.");
                }

                if (fieldPreprocessorSettings.TrimEnd >= value.Length)
                {
                    throw new CsvConfigurationException($"Trimming the field [{value}] would result in its total elimination. Check specific IFieldPreprocessingSettings.TrimEnd settings.");
                }

                value = value.Substring(0, value.Length - fieldPreprocessorSettings.TrimEnd);
            }

            return value;
        }
    }
}