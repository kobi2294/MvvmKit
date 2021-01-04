using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal class ListPadAfterModifier<T> : IListModifier<T>
        where T : class, IImmutable
    {
        private readonly int _count;
        private readonly Func<int, T> _factory;

        public ListPadAfterModifier(int count, Func<int, T> factory)
        {
            _count = count;
            _factory = factory;
            if (_factory == null)
                _factory = i => default;
        }

        public ImmutableList<T> Modify(ImmutableList<T> source)
        {
            if (source.Count >= _count) return source;
            var range = Enumerable.Range(source.Count, _count - source.Count);
            var items = range.Select(_factory);
            return source.AddRange(items);
        }
    }
}
