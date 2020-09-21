using MvvmKit.Tools.Immutables.Fluent;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class Immutables
    {
        private static ConcurrentDictionary<Type, ConstructorInfo> _constructorOfType;

        private static IEnumerable<(string name, Type type)> _parametersOfCtor(ConstructorInfo ci)
        {
            return ci.GetParameters()
                .Select(prm => (name: prm.Name.ToLower(), type: prm.ParameterType));
        }

        private static IEnumerable<(string name, Type type)> _propertiesOfType(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(prop => (name: prop.Name.ToLower(), type: prop.PropertyType));
        }

        private static ConstructorInfo _findCtorInfoOfType(Type type)
        {
            var props = _propertiesOfType(type).ToHashSet();

            var ctor = type
                .GetConstructors()
                .FirstOrDefault(ci => _parametersOfCtor(ci).HasSameElementsAs(props));

            return ctor;
        }

        private static Func<object, object, object> _copyCtorFor(Type type, PropertyInfo prop)
        {
            var ctor =_constructorOfType.GetOrAdd(type, t => _findCtorInfoOfType(t));
            if (ctor == null)
                throw new ArgumentException($"Type {type.Name} does not have a constructor that takes all properties as parameters", nameof(type));
            return ctor.AsCopyConstructor<object, object>(prop);
        }

        private static Func<object> _defaultCreatorFor(Type type)
        {
            var ctor = _constructorOfType.GetOrAdd(type, t => _findCtorInfoOfType(t));
            if (ctor == null)
                throw new ArgumentException($"Type {type.Name} does not have a constructor that takes all properties as parameters", nameof(type));
            return ctor.AsFunc<object>();
        }


        static Immutables()
        {
            _constructorOfType = new ConcurrentDictionary<Type, ConstructorInfo>();
        }

        private static object _with(object source, PropertyInfo prop, object newValue)
        {
            var modifier = _copyCtorFor(source.GetType(), prop);
            var getter = prop.AsGetter<object, object>();
            var oldValue = getter(source);

            if (Equals(oldValue, newValue)) return source;

            var modified = modifier(source, newValue);
            return modified;
        }

        private static object _create(Type type)
        {
            var creator = _defaultCreatorFor(type);
            return creator();
        }

        public static IImmutable With(this IImmutable source, PropertyInfo property, object newValue)
        {
            return _with(source, property, newValue) as IImmutable;
        }

        public static T With<T, TVal>(this T instance, Expression<Func<T, TVal>> property, TVal newValue)
            where T: IImmutable
        {
            return (T)_with(instance, property.GetProperty(), newValue);
        }

        public static T With<T, TVal>(this T instance, Expression<Func<T, TVal>> expression, Func<T, TVal> value)
            where T : class, IImmutable
        {
            return (T)_with(instance, expression.GetProperty(), value(instance));
        }



        public static VersionedList<T> ToVersionedList<T>(this IEnumerable<T> source)
        {
            return VersionedList<T>.Create(source);
        }



        public static ImmutableInstanceWrapper<TRoot, T> With<TRoot, T>(this TRoot source, Expression<Func<TRoot, T>> expression)
            where TRoot : class, IImmutable
            where T : class, IImmutable
        {
            var wrapper = new RootWrapper<TRoot>(source);
            return wrapper.Target.With(expression);
        }

        public static ImmutableListWrapper<TRoot, T> With<TRoot, T>(this TRoot source, Expression<Func<TRoot, ImmutableList<T>>> expression)
            where TRoot : class, IImmutable
            where T : class, IImmutable
        {
            var wrapper = new RootWrapper<TRoot>(source);
            return wrapper.Target.With(expression);
        }

        public static ImmutableInstanceWrapper<TRoot, TRoot> Set<TRoot, T>(this TRoot source, Expression<Func<TRoot, T>> expression, T value)
            where TRoot : class, IImmutable
        {
            var wrapper = new RootWrapper<TRoot>(source);
            wrapper.Target.Set(expression, value);
            return wrapper.Target;
        }

        public static ImmutableInstanceWrapper<TRoot, TRoot> Set<TRoot, T>(this TRoot source, Expression<Func<TRoot, T>> expression, Func<TRoot, T> valueFunc)
            where TRoot : class, IImmutable
        {
            var wrapper = new RootWrapper<TRoot>(source);
            wrapper.Target.Set(expression, valueFunc);
            return wrapper.Target;
        }

        /// <summary>
        /// Creates an instance of an immutable type using reflection.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IImmutable Create(Type type)
        {
            return _create(type) as IImmutable;
        }

        public static T Create<T>()
            where T:IImmutable
        {
            return (T)_create(typeof(T));
        }


        private static IEnumerable<IImmutable> _getAllSubObjectsInProperty(IImmutable obj, PropertyInfo prop)
        {
            if (prop.PropertyType.IsImmutableType())
            {
                var subOjb = prop.GetValue(obj) as IImmutable;
                return subOjb.GetAllSubObjects();
            }

            if (prop.PropertyType.IsEnumreableOfImmutables())
            {
                var list = prop.GetValue(obj) as IEnumerable;
                if (list != null)
                {
                    return list.Cast<IImmutable>().SelectMany(imm => imm.GetAllSubObjects());
                }
            }

            return Enumerable.Empty<IImmutable>();
        }

        /// <summary>
        /// Returns a flatten tree of all the immutable objects under the source object, including itself
        /// </summary>
        public static IEnumerable<IImmutable> GetAllSubObjects(this IImmutable source)
        {
            if (source == null) return Enumerable.Empty<IImmutable>();

            var type = source.GetType();
            var props = type.GetAllProperties();
            var allSubObjects = props.SelectMany(prop => _getAllSubObjectsInProperty(source, prop));

            return source.Yield()
                .Concat(allSubObjects);
        }

        /// <summary>
        /// Reflection helper that returns true if the type implements IImmutable
        /// </summary>
        public static bool IsImmutableType(this Type type)
        {
            return type.GetInterfaces().Contains(typeof(IImmutable));
        }

        /// <summary>
        /// Reflection helper that returns true if the type implements IEnumerable of T where T implements IImmutable
        /// </summary>
        public static bool IsEnumreableOfImmutables(this Type type)
        {
            return type.TryIsEnumerable(out var elementType)
                && elementType.IsImmutableType();
        }

        public static bool IsImmutableListOfImmutables(this Type type)
        {
            if (!type.IsGenericType) return false;
            if (type.IsGenericTypeDefinition) return false;

            var genericDefinition = type.GetGenericTypeDefinition();
            if (genericDefinition != typeof(ImmutableList<>)) return false;

            var elementType = type.GetGenericArguments().First();
            return elementType.IsImmutableType();
        }

        /// <summary>
        /// takes a collection of items and returns a an ImmutableList of them
        /// expectedType is the type of object the caller expects to get, probably ImmutableList of T
        /// </summary>
        private static object _toImmutableList(IEnumerable<IImmutable> items, Type expectedType)
        {
            if (expectedType == null) return new ArgumentNullException(nameof(expectedType));
            if (items == null) return new ArgumentNullException(nameof(items));

            if (!expectedType.IsGenericType) 
                throw new ArgumentException("Generic collection type expected", nameof(expectedType));

            if (expectedType.GetGenericTypeDefinition() == typeof(ImmutableList<>))
            {
                var typeArgument = expectedType.GetGenericArguments()[0];
                var castMethod = typeof(Enumerable).GetMethod("Cast");
                var castTMethod = castMethod.MakeGenericMethod(typeArgument);

                var toImmutableListMethod = typeof(ImmutableList).GetMethod("CreateRange");
                var toImmutableListTMethod = toImmutableListMethod.MakeGenericMethod(typeArgument);

                var casted = castTMethod.Invoke(null, new object[] { items });
                var newList = toImmutableListTMethod.Invoke(null, new object[] { casted });
                return newList;
            }

            throw new ArgumentException("Expecting ImmutableList of T");
        }

        public static IImmutable ModifyRecusively(this IImmutable source, Func<IImmutable, IImmutable> modifier, Type type)
        {
            if (source == null) return null;

            // first check if the source is of the modifiable type and if so apply the modifier on it
            var sourceType = source.GetType();
            if (sourceType.IsInheritedFrom(type))
            {
                source = modifier(source);
            }

            var properties = sourceType.GetAllProperties();

            // now search for all properties that are immutable and apply recursively on them as well
            foreach (var prop in properties)
            {
                if (prop.PropertyType.IsImmutableType())
                {
                    var subObj = prop.GetValue(source) as IImmutable;
                    subObj = ModifyRecusively(subObj, modifier, type);
                    source = source.With(prop, subObj);
                } else if (prop.PropertyType.IsEnumreableOfImmutables())
                {
                    var list = prop.GetValue(source) as IEnumerable;
                    var modified = list
                        .Cast<IImmutable>()
                        .Select(imm => ModifyRecusively(imm, modifier, type));

                    var newList = _toImmutableList(modified, list.GetType());
                    source = source.With(prop, newList);
                }
            }

            return source;
        }

        public static T ModifyRecusively<T, K>(this T source, Func<K, K> modifier)
            where T : IImmutable
            where K : IImmutable
        {
            return (T)ModifyRecusively(source, (IImmutable imm) => modifier((K)imm), typeof(K));
        }

    }
}
