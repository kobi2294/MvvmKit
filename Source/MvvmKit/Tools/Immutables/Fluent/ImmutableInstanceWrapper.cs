using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class ImmutableInstanceWrapper<TRoot, T>: ImmutableWrapper<TRoot>
        where TRoot: class, IImmutable
        where T: class, IImmutable
    {
        public abstract ImmutableListWrapper<TRoot, TObj> With<TObj>(Expression<Func<T, ImmutableList<TObj>>> expression)
            where TObj : class, IImmutable;

        public abstract ImmutableInstanceWrapper<TRoot, TObj> With<TObj>(Expression<Func<T, TObj>> expression)
            where TObj : class, IImmutable;

        public abstract ImmutableInstanceWrapper<TRoot, T> Set<TVal>(Expression<Func<T, TVal>> expression, TVal value, Predicate<T> predicate = null);

        public abstract ImmutableInstanceWrapper<TRoot, T> Set<TVal>(Expression<Func<T, TVal>> expression, Func<T, TVal> valueFunc, Predicate<T> predicate = null);
    }
}
