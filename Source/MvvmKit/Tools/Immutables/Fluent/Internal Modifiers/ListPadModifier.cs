using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal class ListPadModifier<T> : IListModifier<T>
        where T : class, IImmutable
    {
        private readonly int _count;
        private readonly Func<int, T> _factory;
        private readonly bool _isBefore;

        public ListPadModifier(int count, Func<int, T> factory, bool isBefore)
        {
            _count = count;
            _factory = factory;
            _isBefore = isBefore;

            if (_factory == null)
                _factory = i => default;
        }

        public ImmutableList<T> Modify(ImmutableList<T> source)
        {
            if (source.Count >= _count) return source;

            if (_isBefore)
            {
                var range = Enumerable.Range(0, _count - source.Count);
                var items = range.Select(_factory);
                return items.Concat(source).ToImmutableList();
            } else
            {
                var range = Enumerable.Range(source.Count, _count - source.Count);
                var items = range.Select(_factory);
                return source.Concat(items).ToImmutableList();
            }

        }
    }
}
