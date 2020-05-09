using System;
using System.Collections;
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

        /// <summary>
        /// Check if this type implements IEnumerable<T> and if so returns (true, T)
        /// If not, checks if the type implements IEnumerable and if so returns (true, object)
        /// otherwise returns (false, null)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool TryIsEnumerable(this Type type, out Type elementType)
        {
            var interfaces = type.GetInterfaces();

            var ienumerableT = interfaces
                .FirstOrDefault(intr => (intr.IsGenericType)
                                     && (intr.GetGenericTypeDefinition() == typeof(IEnumerable<>)));

            if (ienumerableT != null)
            {
                elementType = ienumerableT.GetGenericArguments()[0];
                return true;
            }

            var ienumerable = interfaces
                .FirstOrDefault(intr => intr == typeof(IEnumerable));

            if (ienumerable != null)
            {
                elementType = typeof(object);
                return true;
            }

            elementType = null;
            return false;
        }

        /// <summary>
        /// returns true if the subType is inherited from suprt type either as class inheritance or interface implementation
        /// </summary>
        public static bool IsInheritedFrom(this Type subType, Type superType)
        {
            return superType.IsAssignableFrom(subType);
        }
    }
}
