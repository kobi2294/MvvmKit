using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class EditableGrouping<K, T> : IGrouping<K, T>
    {
        private K _key;
        private HashSet<T> _values;

        public EditableGrouping(K key)
        {
            _key = key;
            _values = new HashSet<T>();
        }

        public EditableGrouping(K key, T value)
        {
            _key = key;
            _values = new HashSet<T>(value.AsIEnumerable());
        }

        public EditableGrouping(K key, IEnumerable<T> values)
        {
            _key = key;
            _values = new HashSet<T>(values);
        }


        public K Key
        {
            get { return _key; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T value)
        {
            _values.Add(value);
        }

        public void Remove(T value)
        {
            _values.Remove(value);
        }

        public void RemoveWhere(Predicate<T> predicate)
        {
            _values.RemoveWhere(predicate);
        }

        public int Count
        {
            get
            {
                return _values.Count;
            }
        }

        public bool ContainsValue(T value)
        {
            return _values.Contains(value);
        }

        public void Reset(IEnumerable<T> values)
        {
            _values = new HashSet<T>(values);
        }

        public void Reset(params T[] values)
        {
            _values = new HashSet<T>(values);
        }
    }
}
