using MvvmKit.CollectionChangeEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class CollectionChanges<T> : IEnumerable<IChange<T>>
    {
        private List<IChange<T>> _changes;

        public CollectionChanges(IEnumerable<IChange<T>> changes)
        {
            _changes = new List<IChange<T>>(changes);
        }

        public CollectionChanges()
        {
            _changes = new List<IChange<T>>();
        }

        public int Count => _changes.Count;

        public IEnumerator<IChange<T>> GetEnumerator()
        {
            return _changes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static implicit operator CollectionChanges<T>(ItemAdded<T> change)
        {
            return new CollectionChanges<T>(change.AsIEnumerable());
        }

        public static implicit operator CollectionChanges<T>(ItemRemoved<T> change)
        {
            return new CollectionChanges<T>(change.AsIEnumerable());
        }

        public static implicit operator CollectionChanges<T>(ItemMoved<T> change)
        {
            return new CollectionChanges<T>(change.AsIEnumerable());
        }

        public static implicit operator CollectionChanges<T>(ItemReplaced<T> change)
        {
            return new CollectionChanges<T>(change.AsIEnumerable());
        }

        public static implicit operator CollectionChanges<T>(Cleared<T> change)
        {
            return new CollectionChanges<T>(change.AsIEnumerable());
        }

        public static implicit operator CollectionChanges<T>(Reset<T> change)
        {
            return new CollectionChanges<T>(change.AsIEnumerable());
        }
    }
}
