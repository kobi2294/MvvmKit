﻿using System;
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

        public abstract ImmutableInstanceWrapper<TRoot, T> Set<TVal>(Expression<Func<T, TVal>> expression, TVal value);

        public abstract ImmutableInstanceWrapper<TRoot, T> Set<TVal>(Expression<Func<T, TVal>> expression, Func<T, TVal> valueFunc);

        public abstract ImmutableInstanceWrapper<TRoot, T> Replace(T value);

        public abstract ImmutableInstanceWrapper<TRoot, T> Replace(Func<T, T> value);

        public abstract ImmutableInstanceWrapper<TRoot, T> If(Predicate<T> predicate);

        public abstract ImmutableInstanceWrapper<TRoot, TCast> Cast<TCast>()
            where TCast : class, IImmutable;

    }
}
