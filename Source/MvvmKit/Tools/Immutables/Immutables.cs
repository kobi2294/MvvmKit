using Remutable;
using System;
using System.Collections.Generic;
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
    }
}
