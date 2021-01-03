using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    public class ListUpsertModifier<T, K> : IListModifier<T>
        where T: class, IImmutable
    {
        private T[] _items;
        private Func<T, K> _keySelector;

        public ListUpsertModifier(T[] items, Func<T, K> keySelector)
        {
            _items = items;
            _keySelector = keySelector;
        }

        public ImmutableList<T> Modify(ImmutableList<T> source)
        {
            var newItemsByKey = _items.ToDictionary(_keySelector);
            var sourceItemsByKey = source.ToDictionary(_keySelector);

            var toAdd = newItemsByKey
                .Keys
                .Except(sourceItemsByKey.Keys)
                .Select(key => newItemsByKey[key])
                .ToArray();

            return source
                .Select(item => _keySelector(item))
                .Select(key => newItemsByKey.ContainsKey(key) ? newItemsByKey[key] : sourceItemsByKey[key])
                .Concat(toAdd)
                .ToImmutableList();
        }
    }
}
