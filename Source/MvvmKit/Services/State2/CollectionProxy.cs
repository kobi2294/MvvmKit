using MvvmKit.CollectionChangeEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class CollectionProxy<T> : IStateList<T>
    {
        private readonly object _syncRoot = new object();
        private readonly List<T> _list;
        private readonly Action<IChange> _onChange;
        private readonly bool _modifiable = false;
        private readonly AccessInterceptor _interceptor;

        public CollectionProxy(
            List<T> list, 
            AccessInterceptor interceptor, 
            Action<IChange> onChange, 
            bool modifiable)
        {
            _interceptor = interceptor;
            _list = list;
            _onChange = onChange;
            _modifiable = modifiable;
        }

        private void _notify(IChange<T> change)
        {
            _onChange.Invoke(change);
        }

        private void _notify(IEnumerable<IChange<T>> changes)
        {
            foreach (var change in changes)
            {
                _onChange.Invoke(change);
            }
        }

        private void _verifyWrite()
        {
            if (_interceptor.IsDisposed)
            {
                throw new ObjectDisposedException("Collection proxy was disposed");
            }

            if (!_modifiable)
                throw new InvalidOperationException("Attempting to modify a read-onlu collection");
        }

        private void _verifyRead()
        {
            if (_interceptor.IsDisposed)
            {
                throw new ObjectDisposedException("Collection proxy was disposed");
            }
        }


        #region IStateCollection<T>

        public T this[int index]
        {
            get
            {
                _verifyRead();
                return _list[index];
            }
            set
            {
                _verifyWrite();
                var oldItem = _list[index];
                _list[index] = value;
                _notify(Changes.Replace(index, oldItem, value));
            }
        }

        public int Count {
            get
            {
                _verifyRead();
                return _list.Count;
            }
        }

        public bool IsReadOnly => !_modifiable;

        public bool IsFixedSize => false;

        public object SyncRoot => _syncRoot;

        public bool IsSynchronized => false;

        public void Add(T item)
        {
            _verifyWrite();
            _list.Add(item);
            _notify(Changes.Add(_list.Count - 1, item));

        }

        public int Add(object value)
        {
            Add((T)value);
            return _list.Count - 1;
        }

        public void Clear()
        {
            _verifyWrite();
            _list.Clear();
            _notify(Changes.Clear<T>());
        }

        public bool Contains(T item)
        {
            _verifyRead();
            return _list.Contains(item);
        }

        public bool Contains(object value)
        {
            _verifyRead();
            return _list.Contains((T)value);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _verifyRead();
            _list.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            _verifyRead();
            T[] tarray = array as T[];
            _list.CopyTo(tarray, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            _verifyRead();
            return _list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            _verifyRead();
            return _list.IndexOf(item);
        }

        public int IndexOf(object value)
        {
            _verifyRead();
            return _list.IndexOf((T)value);
        }

        public void Insert(int index, T item)
        {
            _verifyWrite();
            _list.Insert(index, item);
            _notify(Changes.Add(index, item));
        }

        public void Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        public bool Remove(T item)
        {
            _verifyWrite();
            var index = _list.IndexOf(item);
            if (index > -1)
            {
                _list.RemoveAt(index);
                _notify(Changes.Remove(index, item));
                return true;
            }

            return false;
        }

        public void Remove(object value)
        {
            Remove((T)value);
        }

        public void RemoveAt(int index)
        {
            _verifyWrite();
            var item = _list[index];
            _list.RemoveAt(index);
            _notify(Changes.Remove(index, item));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            _verifyRead();
            return _list.GetEnumerator();
        }

        public bool Exists(Predicate<T> predicate)
        {
            _verifyRead();
            return _list.Exists(predicate);
        }

        public T Find(Predicate<T> predicate)
        {
            _verifyRead();
            return _list.Find(predicate);
        }

        public int FindIndex(Predicate<T> predicate)
        {
            _verifyRead();
            return _list.FindIndex(predicate);
        }

        public void SetWhere(Predicate<T> predicate, T item)
        {
            var index = _list.IndexOf(t => predicate(t));
            if (index >= 0)
            {
                this[index] = item;
            }
        }

        public void AddRange(IEnumerable<T> values)
        {
            _verifyWrite();
            var start = _list.Count;
            _list.AddRange(values);

            _notify(values.Select((v, i) => Changes.Add(start + i, v)));
        }

        public void AddRange(params T[] values)
        {
            _verifyWrite();
            var start = _list.Count;
            _list.AddRange(values);

            _notify(values.Select((v, i) => Changes.Add(start + i, v)));
        }

        public void InsertRange(int index, IEnumerable<T> values)
        {
            _verifyWrite();
            var start = index;
            _list.InsertRange(index, values);
            _notify(values.Select((v, i) => Changes.Add(start + i, v)));
        }
        public void InsertRange(int index, params T[] values)
        {
            _verifyWrite();
            var start = index;
            _list.InsertRange(index, values);
            _notify(values.Select((v, i) => Changes.Add(start + i, v)));
        }


        public void MoveAt(int oldIndex, int newIndex)
        {
            _verifyWrite();
            T item = _list[oldIndex];
            _list.RemoveAt(oldIndex);
            _list.Insert(newIndex, item);
            _notify(Changes.Move(oldIndex, newIndex, item));
        }

        public void MoveItem(T item, int newIndex)
        {
            var oldIndex = _list.IndexOf(item);
            if (oldIndex >= 0) MoveAt(oldIndex, newIndex);
        }

        public void RemoveWhere(Predicate<T> predicate)
        {
            _verifyWrite();
            var itemsToRemove = _list
                .Select((v, i) => (index: i, value: v))
                .Where(pair => predicate(pair.value))
                .OrderByDescending(pair => pair.index)
                .Select(pair => Changes.Remove(pair.index, pair.value))
                .ToList();

            foreach (var pair in itemsToRemove)
            {
                _list.RemoveAt(pair.Index);
            }

            _notify(itemsToRemove);
        }

        public void Reset(IEnumerable<T> values)
        {
            _verifyWrite();
            _list.Clear();
            _list.AddRange(values);
            _notify(Changes.Reset(values));
        }

        public void Reset(params T[] values)
        {
            _verifyWrite();
            _list.Clear();
            _list.AddRange(values);
            _notify(Changes.Reset(values));
        }

        #endregion
    }
}
