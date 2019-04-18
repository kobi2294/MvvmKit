using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class EditableLookup<K, T> : ILookup<K, T>
    {
        private readonly LazyDictionary<K, EditableGrouping<K, T>> _groups;

        public EditableLookup()
        {
            _groups = new LazyDictionary<K, EditableGrouping<K, T>>(key => new EditableGrouping<K, T>(key));
        }

        public bool Contains(K key)
        {
            return _groups.ContainsKey(key);
        }

        public bool ContainsPair(K key, T value)
        {
            return _groups.ContainsKey(key)
                        && _groups[key].ContainsValue(value);
        }

        public int Count
        {
            get { return _groups.Count; }
        }

        public IEnumerable<T> this[K key]
        {
            get
            {
                if (_groups.ContainsKey(key))
                    return _groups[key];

                return Enumerable.Empty<T>();
            }
        }

        public IEnumerator<IGrouping<K, T>> GetEnumerator()
        {
            return _groups.Select(kvp => kvp.Value).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(K key, T value)
        {
            var group = _groups[key];
            group.Add(value);
        }

        public void Reset(K key, IEnumerable<T> values)
        {
            var group = _groups[key];
            group.Reset(values);
        }

        public void Reset(K key, params T[] values)
        {
            var group = _groups[key];
            group.Reset(values);
        }

        public void Remove(K key, T value)
        {
            if (_groups.ContainsKey(key))
            {
                var group = _groups[key];
                group.Remove(value);

                // if bucket is empty, remove it altogether
                if (group.Count == 0) _groups.Remove(key);
            }
        }

        public void RemoveKey(K key)
        {
            _groups.Remove(key);
        }

        public void RemoveWhere(K key, Predicate<T> predicate)
        {
            _groups[key].RemoveWhere(predicate);
            if (_groups[key].Count == 0) RemoveKey(key);
        }

        public void Clear()
        {
            _groups.Clear();
        }

        public IEnumerable<K> Keys
        {
            get
            {
                return _groups.Select(kvp => kvp.Key);
            }
        }

    }
}
