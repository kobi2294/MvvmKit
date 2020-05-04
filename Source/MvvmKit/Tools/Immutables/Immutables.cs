using MvvmKit.Tools.Immutables.Fluent;
using Remutable;
using System;
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
            where T : IImmutable
        {
            _verifyRemute<T>();
            return _remute.With(instance, expression, value);
        }

        public static T With<T, TVal>(this T instance, Expression<Func<T, TVal>> expression, Func<T, TVal> value)
            where T : IImmutable
        {
            _verifyRemute<T>();
            return _remute.With(instance, expression, value(instance));
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
    }
}
