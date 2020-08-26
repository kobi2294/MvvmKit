using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal class ListRemoveModifier<T>: IListModifier<T>
        where T: class, IImmutable
    {
        private readonly Predicate<T>[] _predicates;
        private readonly Func<T, int, bool> _indexedPredicate;
        private readonly bool _usesIndex = false;

        public ListRemoveModifier(Predicate<T>[] predicates)
        {
            _predicates = predicates;
            _indexedPredicate = null;
            _usesIndex = false;
        }

        public ListRemoveModifier(Func<T, int, bool> predicate)
        {
            _indexedPredicate = predicate;
            _predicates = null;
            _usesIndex = true;
        }

        ImmutableList<T> IListModifier<T>.Modify(ImmutableList<T> source)
        {
            var current = source;

            if (_usesIndex)
            {
                var toRemove = source.Where(_indexedPredicate);
                current = current.RemoveRange(toRemove);
            } else
            {
                foreach (var predicate in _predicates)
                {
                    current = current.RemoveAll(predicate);
                }
            }

            return current;
        }
    }
}
