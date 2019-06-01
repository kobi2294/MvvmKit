using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class CollectionChanges<T> : IEnumerable<ICollectionChange<T>>
    {
        private List<ICollectionChange<T>> _items;

        public CollectionChanges(IEnumerable<ICollectionChange<T>> items)
        {
            _items = new List<ICollectionChange<T>>(items);
        }

        public CollectionChanges()
        {
            _items = new List<ICollectionChange<T>>();
        }

        public int Count => _items.Count;

        public IEnumerator<ICollectionChange<T>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
