using System;
using System.Linq.Expressions;

namespace MvvmKit
{
    public static class CommonEnsurers
    {
        public static T EnsureNotDefault<T, K>(this T source, Expression<Func<T, K>> prop, Func<K> valueIfDefault)
            where T : class, IImmutable
        {
            var getter = prop.AsGetter();
            var value = getter(source);
            if (Equals(value, default))
            {
                return source.With(prop, valueIfDefault());
            } else
            {
                return source;
            }
        }
    }
}
