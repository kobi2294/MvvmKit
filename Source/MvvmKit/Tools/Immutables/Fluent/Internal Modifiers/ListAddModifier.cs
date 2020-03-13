using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal class ListAddModifier<T>: IListModifier<T>
        where T: class, IImmutable
    {
        private readonly T[] _items;

        public ListAddModifier(T[] items)
        {
            _items = items;
        }

        public ImmutableList<T> Modify(ImmutableList<T> source)
        {
            return source.AddRange(_items);
        }
    }
}
