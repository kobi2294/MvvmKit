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

        public ListRemoveModifier(Predicate<T>[] predicates)
        {
            _predicates = predicates;
        }

        ImmutableList<T> IListModifier<T>.Modify(ImmutableList<T> source)
        {
            var current = source;
            foreach (var predicate in _predicates)
            {
                current = current.RemoveAll(predicate);
            }
            return current;
        }
    }
}
