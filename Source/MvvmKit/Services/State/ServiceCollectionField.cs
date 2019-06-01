using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ServiceCollectionField<T>
    {
        private List<T> _items;

        public ServiceCollectionField(IEnumerable<T> values = null)
        {
            if (values != null)
            {
                _items = new List<T>(values);
            } else
            {
                _items = new List<T>();
            }
        }

        public T this[int index] => _items[index];

        public int Count => _items.Count;

        public bool Contains(T item) => _items.Contains(item);

        public bool Exists(Predicate<T> predicate) => _items.Exists(predicate);

        public T Find(Predicate<T> predicate) => _items.Find(predicate);

        public List<T> FindAll(Predicate<T> predicate) => _items.FindAll(predicate);

        public int FindIndex(Predicate<T> predicate) => _items.FindIndex(predicate);

        public List<T> GetRange(int index, int count) => _items.GetRange(index, count);

        public List<T> Items => _items.ToList();

        public int IndexOf(T item) => _items.IndexOf(item);


        public async Task Add(T value)
        {
            _items.Add(value);
        }

        public async Task AddRange(IEnumerable<T> value)
        {
        }

        public async Task Clear()
        {
        }

        public async Task Insert(int index, T item)
        {
        }

        public async Task InsertRange(int index, IEnumerable<T> collection)
        {
        }

        public async Task Remove(T item) { }

        public async Task RemoveAll(Predicate<T> match) { }

        public async Task RemoveAt(int index) { }

        public async Task Reset(IEnumerable<T> values) { }

        public async Task Transform(IEnumerable<T> values) { }

    }
}
