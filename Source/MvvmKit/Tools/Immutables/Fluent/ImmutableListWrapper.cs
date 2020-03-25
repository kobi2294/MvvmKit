﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class ImmutableListWrapper<TRoot, T>: ImmutableWrapper<TRoot>
        where TRoot: class, IImmutable
        where T: class, IImmutable
    {
        public abstract ImmutableInstanceWrapper<TRoot, T> Find(Predicate<T> predicate);

        public abstract ImmutableListWrapper<TRoot, T> Add(params T[] items);

        public abstract ImmutableListWrapper<TRoot, T> Remove(params Predicate<T>[] predicates);
    }
}