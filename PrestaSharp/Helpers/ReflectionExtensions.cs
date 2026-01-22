using System;
using System.Reflection;

namespace RestSharp.Extensions
{
    /// <summary>
    /// Extension methods for reflection to replace removed RestSharp functionality
    /// </summary>
    internal static class ReflectionExtensions
    {
        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttribute<T>();
        }

        public static T GetAttribute<T>(this PropertyInfo propertyInfo) where T : Attribute
        {
            return propertyInfo.GetCustomAttribute<T>();
        }

        public static T GetAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return memberInfo.GetCustomAttribute<T>();
        }
    }
}
