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
        public static T With<T, TVal>(this T instance, Expression<Func<T, TVal>> expression, TVal value)
            where T : IImmutable
        {
            return Remute.Default.With(instance, expression, value);
        }

        public static T With<T, TVal>(this T instance, Expression<Func<T, TVal>> expression, Func<T, TVal> value)
            where T : IImmutable
        {
            return Remute.Default.With(instance, expression, value(instance));
        }
    }
}
