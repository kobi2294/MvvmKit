using MvvmKit.Tools.Immutables.Fluent;
using Remutable;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class Immutables
    {
        private static HashSet<Type> _verified = new HashSet<Type>();
        private static Remute _remute;
        private static ActivationConfiguration _config;

        private static void _verifyRemute<T>()
        {
            if (_verified.Contains(typeof(T))) return;

            var ctor = typeof(T).GetConstructors()
                .OrderByDescending(xtor => xtor.GetParameters().Length)
                .First();

            _config.Configure(ctor);
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
    }
}
