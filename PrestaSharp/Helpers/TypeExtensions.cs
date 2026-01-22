using System;
using System.Linq;

namespace RestSharp.Extensions
{
    /// <summary>
    /// Extension methods for Type to replace removed RestSharp functionality
    /// </summary>
    internal static class TypeExtensions
    {
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static object FindEnumValue(this Type type, string value, System.Globalization.CultureInfo culture)
        {
            var ret = Enum.GetValues(type)
                .Cast<Enum>()
                .FirstOrDefault(v => v.ToString().Equals(value, StringComparison.OrdinalIgnoreCase));

            if (ret == null)
            {
                var enumValueAsUnderlyingType = Convert.ChangeType(value, Enum.GetUnderlyingType(type), culture);
                if (enumValueAsUnderlyingType != null && Enum.IsDefined(type, enumValueAsUnderlyingType))
                {
                    ret = (Enum)Enum.ToObject(type, enumValueAsUnderlyingType);
                }
            }

            return ret;
        }
    }
}
