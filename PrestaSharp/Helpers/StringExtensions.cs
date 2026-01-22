using System;
using System.Globalization;
using System.Xml.Linq;

namespace Bukimedia.PrestaSharp.Helpers
{
    /// <summary>
    /// Extension methods for strings to replace removed RestSharp functionality
    /// </summary>
    internal static class StringExtensions
    {
        public static XName AsNamespaced(this string name, string @namespace)
        {
            return string.IsNullOrEmpty(@namespace) ? XName.Get(name) : XName.Get(name, @namespace);
        }

        public static string ToCamelCase(this string value, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value) || value.Length < 2)
                return value;

            return char.ToLower(value[0], culture) + value.Substring(1);
        }

        public static string RemoveUnderscoresAndDashes(this string value)
        {
            return value?.Replace("_", "").Replace("-", "");
        }

        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
