using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class ReflectionExtensions
    {
        private static IEnumerable<PropertyInfo> _allPropertiesOfClass(this Type type)
        {
            return type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
        }

        private static IEnumerable<PropertyInfo> _allPRopertiesOfInterface(this Type type)
        {
            return type.Yield()
                .Concat(type.GetInterfaces())
                .SelectMany(i => i.GetProperties());
        }


        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            if (type.IsInterface)
            {
                return _allPRopertiesOfInterface(type);
            } else
            {
                return _allPropertiesOfClass(type);
            }
        }

        public static object DefaultValue(this Type type)
        {
            if (type.IsValueType) return Activator.CreateInstance(type);
            return null;
        }

        public static bool IsGenericOf(this Type type, Type genericDefinition)
        {
            if (!genericDefinition.IsGenericTypeDefinition)
                throw new ArgumentException("Must be a generic definition", nameof(genericDefinition));

            return
                type.IsGenericType
                && type.GetGenericTypeDefinition() == genericDefinition;
        }
    }
}
