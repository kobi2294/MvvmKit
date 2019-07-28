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
        private readonly IReadOnlyList<T> _oldValues;
        private readonly IReadOnlyList<T> _newValues;

        public CollectionChanges(IEnumerable<IChange<T>> changes, IEnumerable<T> oldValues, IEnumerable<T> newValues)
        {
            _changes = new List<IChange<T>>(changes);
            _oldValues = oldValues.ToReadOnly();
            _newValues = newValues.ToReadOnly();
        }

        public CollectionChanges(IChange[] changes, IEnumerable oldvals, IEnumerable newVals)
            :this(changes.Cast<IChange<T>>(), oldvals.Cast<T>(), newVals.Cast<T>())
        {
        }
        
        public CollectionChanges()
        {
            _changes = new List<IChange<T>>();
        }

        public IReadOnlyList<T> OldValues => _oldValues;
        public IReadOnlyList<T> NewValues => _newValues;

        public int Count => _changes.Count;

        public IEnumerator<IChange<T>> GetEnumerator()
        {
            return _changes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
