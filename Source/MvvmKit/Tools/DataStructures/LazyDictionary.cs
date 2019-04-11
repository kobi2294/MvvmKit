using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class LazyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _items;
        private Func<TKey, TValue> _factory;

        private Dictionary<TKey, TValue> _getItems()
        {
            if (_items == null) _items = new Dictionary<TKey, TValue>();
            return _items;
        }

        private ICollection<KeyValuePair<TKey, TValue>> _getKewValueCollection()
        {
            return (_getItems() as ICollection<KeyValuePair<TKey, TValue>>);
        }

        private TValue _getItem(TKey key)
        {
            var items = _getItems();
            if (!items.ContainsKey(key))
            {
                var value = _factory(key);
                items.Add(key, value);
            }

            return items[key];
        }

        public LazyDictionary(Func<TKey, TValue> factory)
        {
            _factory = factory;
        }


        public TValue this[TKey key] {
            get => _getItem(key);
            set => _getItems()[key] = value;
        }

        public ICollection<TKey> Keys => _getItems().Keys;

        public ICollection<TValue> Values => _getItems().Values;

        public int Count => _items != null ? _items.Count : 0;

        public bool IsReadOnly => _getKewValueCollection().IsReadOnly;

        public void Add(TKey key, TValue value)
        {
            _getItems().Add(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _getItems().Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _getItems().Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _getItems().Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return _getItems().ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _getKewValueCollection().CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _getKewValueCollection().GetEnumerator(); 
        }

        public bool Remove(TKey key)
        {
            return _getItems().Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _getKewValueCollection().Remove(item);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _getItems().TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _getKewValueCollection().GetEnumerator();
        }
    }
}
