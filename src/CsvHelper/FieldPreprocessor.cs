namespace CsvHelper
{
    using System.Text.RegularExpressions;
    using Configuration;

    internal static class FieldPreprocessor
    {
        public static string ProcessField(string value, IFieldPreprocessingSettings settings)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (settings.RegexMatchPattern != null)
            {
                if (settings.ExpectedPrefix != null || settings.ExpectedSuffix != null || settings.TrimStart > 0 || settings.TrimEnd > 0)
                {
                    throw new CsvConfigurationException("Regex replacement cannot be used in conjunction with other preprocessing methods.");
                }

                value = Regex.Replace(value, settings.RegexMatchPattern, settings.RegexReplacementPattern);

                return value;
            }

            if (settings.ExpectedPrefix != null)
            {
                if (!value.StartsWith(settings.ExpectedPrefix))
                {
                    throw new CsvBadDataException($"Expected field prefix {settings.ExpectedPrefix} in ...");
                }

                if (settings.TrimPrefix)
                {
                    value = value.Remove(0, settings.ExpectedPrefix.Length);
                }
            }

            if (settings.ExpectedSuffix != null)
            {
                if (!value.EndsWith(settings.ExpectedSuffix))
                {
                    throw new CsvBadDataException($"Expected field suffix {settings.ExpectedSuffix} in ...");
                }

                if (settings.TrimSuffix)
                {
                    value = value.Substring(0, value.Length - settings.ExpectedSuffix.Length);
                }
            }

            if (settings.TrimStart > 0)
            {
                if (settings.TrimPrefix)
                {
                    throw new CsvConfigurationException("Field preprocessing settings cannot have both TrimPrefix and TrimStart flags set.");
                }

                if (settings.TrimStart >= value.Length)
                {
                    throw new CsvConfigurationException($"Trimming the field [{value}] would result in its total elimination. Check specific IFieldPreprocessingSettings.TrimStart settings.");
                }

                value = value.Remove(0, settings.TrimStart);
            }

            if (settings.TrimEnd > 0)
            {
                if (settings.TrimSuffix)
                {
                    throw new CsvConfigurationException("Field preprocessing settings cannot have both TrimSuffix and TrimEnd flags set.");
                }

                if (settings.TrimEnd >= value.Length)
                {
                    throw new CsvConfigurationException($"Trimming the field [{value}] would result in its total elimination. Check specific IFieldPreprocessingSettings.TrimEnd settings.");
                }

                value = value.Substring(0, value.Length - settings.TrimEnd);
            }

            return value;
        }
    }
}