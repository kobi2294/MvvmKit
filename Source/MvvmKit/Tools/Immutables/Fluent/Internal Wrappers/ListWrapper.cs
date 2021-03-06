﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal class ListWrapper<TRoot, T>: ImmutableListWrapper<TRoot, T>, IListModifier<T>
        where TRoot: class, IImmutable
        where T: class, IImmutable
    {
        private readonly List<IListModifier<T>> _modifiers;
        private readonly RootWrapper<TRoot> _root;

        public ListWrapper(RootWrapper<TRoot> root)
        {
            _modifiers = new List<IListModifier<T>>();
            _root = root;
        }

        public ImmutableList<T> Modify(ImmutableList<T> source)
        {
            var current = source;
            foreach (var modifier in _modifiers)
            {
                current = modifier.Modify(current);
            }
            return current;
        }

        public override ImmutableInstanceWrapper<TRoot, T> Find(Predicate<T> predicate)
        {
            var modifier = new ListFindInstanceModifier<TRoot, T>(_root, predicate);
            _modifiers.Add(modifier);
            return modifier.Target;
        }

        public override ImmutableInstanceWrapper<TRoot, T> At(int index)
        {
            var modifier = new ListAtInstanceModifier<TRoot, T>(_root, index);
            _modifiers.Add(modifier);
            return modifier.Target;
        }


        public override ImmutableListWrapper<TRoot, T> Add(params T[] items)
        {
            var modifier = new ListAddModifier<T>(items);
            _modifiers.Add(modifier);
            return this;
        }

        public override ImmutableListWrapper<TRoot, T> PadAfter(int count, Func<int, T> factory = null)
        {
            var modifier = new ListPadModifier<T>(count, factory, isBefore: false);
            _modifiers.Add(modifier);
            return this;
        }
        public override ImmutableListWrapper<TRoot, T> PadBefore(int count, Func<int, T> factory = null)
        {
            var modifier = new ListPadModifier<T>(count, factory, isBefore: true);
            _modifiers.Add(modifier);
            return this;
        }

        public override ImmutableListWrapper<TRoot, T> AddRange(IEnumerable<T> items)
        {
            var modifier = new ListAddModifier<T>(items.ToArray());
            _modifiers.Add(modifier);
            return this;
        }


        public override ImmutableListWrapper<TRoot, T> Remove(Predicate<T> predicate)
        {
            var modifier = new ListRemoveModifier<T>(predicate);
            _modifiers.Add(modifier);
            return this;
        }

        public override ImmutableListWrapper<TRoot, T> Remove(Func<T, int, bool> predicate)
        {
            var modifier = new ListRemoveModifier<T>(predicate);
            _modifiers.Add(modifier);
            return this;
        }

        public override ImmutableListWrapper<TRoot, T> Replace(Func<T, int, T> projection)
        {
            var modifier = new ListReplaceModifier<T>(projection);
            _modifiers.Add(modifier);
            return this;
        }

        public override ImmutableListWrapper<TRoot, T> Replace(Func<T, T> projection)
        {
            var modifier = new ListReplaceModifier<T>(projection);
            _modifiers.Add(modifier);
            return this;
        }

        public override ImmutableListWrapper<TRoot, T> Upsert<K>(IEnumerable<T> items, Func<T, K> keySelector)
        {
            var modifier = new ListUpsertModifier<T, K>(items.ToArray(), keySelector);
            _modifiers.Add(modifier);
            return this;
        }

        public override TRoot Go()
        {
            return _root.Go();
        }

    }
}
