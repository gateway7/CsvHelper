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
                if (fieldPreprocessorSettings.ExpectedPrefix != null || fieldPreprocessorSettings.ExpectedSuffix != null || fieldPreprocessorSettings.TrimStart > 0 || fieldPreprocessorSettings.TrimEnd > 0)
                {
                    throw new CsvConfigurationException("Regex replacement cannot be used in conjunction with other preprocessing methods.");
                }

                value = Regex.Replace(value, fieldPreprocessorSettings.RegexMatchPattern, fieldPreprocessorSettings.RegexReplacementPattern);

                return value;
            }

            if (fieldPreprocessorSettings.ExpectedPrefix != null)
            {
                if (!value.StartsWith(fieldPreprocessorSettings.ExpectedPrefix))
                {
                    throw new CsvBadDataException($"Expected field prefix {fieldPreprocessorSettings.ExpectedPrefix} in ...");
                }

                if (fieldPreprocessorSettings.TrimPrefix)
                {
                    value = value.Remove(0, fieldPreprocessorSettings.ExpectedPrefix.Length);
                }
            }

            if (fieldPreprocessorSettings.ExpectedSuffix != null)
            {
                if (!value.EndsWith(fieldPreprocessorSettings.ExpectedSuffix))
                {
                    throw new CsvBadDataException($"Expected field suffix {fieldPreprocessorSettings.ExpectedSuffix} in ...");
                }

                if (fieldPreprocessorSettings.TrimSuffix)
                {
                    value = value.Substring(0, value.Length - fieldPreprocessorSettings.ExpectedSuffix.Length);
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