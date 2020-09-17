using MvvmKit.Tools.Immutables.Fluent;
using Remutable;
using System;
using System.Collections;
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
        private static HashSet<Type> _verified = new HashSet<Type>();
        private static Remute _remute;
        private static ActivationConfiguration _config;
        private static Dictionary<Type, ConstructorInfo> _ctors = new Dictionary<Type, ConstructorInfo>();
        private static object _mutex = new object();

        private static void _verifyRemute<T>()
        {
            lock(_mutex)
            {
                if (_verified.Contains(typeof(T))) return;
                _verified.Add(typeof(T));

                var ctor = typeof(T).GetConstructors()
                    .OrderByDescending(xtor => xtor.GetParameters().Length)
                    .First();

                _config.Configure(ctor);
                _ctors.Add(typeof(T), ctor);
            }
        }

        private static void _verifyRemute(Type type)
        {
            lock (_mutex)
            {
                if (_verified.Contains(type)) return;
                _verified.Add(type);

                var ctor = type.GetConstructors()
                    .OrderByDescending(xtor => xtor.GetParameters().Length)
                    .First();

                _config.Configure(ctor);
                _ctors.Add(type, ctor);
            }
        }


        static Immutables()
        {
            _config = new ActivationConfiguration();
            _remute = new Remute(_config);
        }


        public static T With<T, TVal>(this T instance, Expression<Func<T, TVal>> expression, TVal value)
            where T : class, IImmutable
        {
            if (instance == null) return default;
            _verifyRemute(instance.GetType());
            return _with(instance, expression, value);
        }

        public static T With<T, TVal>(this T instance, Expression<Func<T, TVal>> expression, Func<T, TVal> value)
            where T : class, IImmutable
        {
            if (instance == null) return default;
            _verifyRemute(instance.GetType());
            return _with(instance, expression, value(instance));
        }

        private static T _with<T, TVal>(T instance, Expression<Func<T, TVal>> expression, TVal value)
            where T : class, IImmutable
        {
            if (typeof(T) == instance.GetType())
            {
                return _remute.With(instance, expression, value);
            } else
            {
                var propName = expression.GetName();
                var prop = instance.GetType().GetProperty(propName);
                return instance.With(prop, value) as T;
            }
        }

        public static IImmutable With(this IImmutable instance, PropertyInfo prop, object value)
        {
            if (instance == null) return null;
            if (prop == null) throw new ArgumentNullException(nameof(prop));

            var instanceType = instance.GetType();
            _verifyRemute(instanceType);

            var propType = prop.PropertyType;

            var propertyExpression = prop.ToFuncExpression(instanceType);
            var withGenericMethod = typeof(Remute).GetMethods()
                .Single(m => (m.Name == nameof(With))
                            && (m.IsGenericMethodDefinition)
                            && (m.GetParameters().Length == 3));

            var withMethodInfo = withGenericMethod.MakeGenericMethod(instanceType, propType);
            var withFunc = withMethodInfo.ToFunc<Remute, object, LambdaExpression, object, object>();

            var res = withFunc(_remute, instance, propertyExpression, value);
            return res as IImmutable;
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
            _verifyRemute(type);
            ConstructorInfo ctor = null;
            lock(_mutex)
            {
                ctor = _ctors[type];
            }

            var paramsCount = ctor.GetParameters().Length;
            var prms = Enumerable.Repeat(Type.Missing, paramsCount).ToArray();

            var value = ctor.Invoke(
                BindingFlags.OptionalParamBinding |
                BindingFlags.InvokeMethod |
                BindingFlags.CreateInstance 
                , null, prms , CultureInfo.CurrentCulture) as IImmutable;

            return value;
        }


        private static IEnumerable<IImmutable> _getAllSubOjectsInProperty(IImmutable obj, PropertyInfo prop)
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
            var allSubObjects = props.SelectMany(prop => _getAllSubOjectsInProperty(source, prop));

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
