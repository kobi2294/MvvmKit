using MvvmKit.CollectionChangeEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ServiceCollectionField<T>
    {
        private List<T> _items;

        private ServiceCollectionPropertyBase<T> _prop;

        public AsyncEvent<CollectionChanges<T>> Changed { get; }



        public ServiceCollectionField(IEnumerable<T> values = null)
        {
            if (values != null)
            {
                _items = new List<T>(values);
            } else
            {
                _items = new List<T>();
            }

            Changed = new AsyncEvent<CollectionChanges<T>>(Changes.Init(_items))
                // instead of returning the latest change, on subscribe, we return a reset to the entire list
                .OnSubscribe(async cb =>
                {
                    await cb.Invoke(Changes.Init(_items));
                });
        }

        public ServiceCollectionField(params T[] values)
            :this(values.AsEnumerable())
        {
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


        public async Task SetAt(int index, T item)
        {
            var oldVals = _items.ToArray();
            var oldItem = _items[index];
            _items[index] = item;
            await Changed.Invoke(Changes.Replace(index, oldItem, item).Collect(oldVals, _items));
        }

        public async Task SetWhere(Predicate<T> predicate, T item)
        {
            var index = _items.IndexOf(t => predicate(t));

            if (index >= 0)
            {
                await SetAt(index, item);
            }
        }

        public async Task Add(T value)
        {
            var oldVals = _items.ToArray();
            _items.Add(value);
            await Changed.Invoke(Changes.Add(_items.Count - 1, value).Collect(oldVals, _items));
        }

        public async Task AddRange(IEnumerable<T> values)
        {
            var oldVals = _items.ToArray();
            var start = _items.Count;
            _items.AddRange(values);

            var changes = values.Select((v, i) => Changes.Add(start + i, v));
            await Changed.Invoke(changes.Collect(oldVals, _items));
        }

        public async Task Clear()
        {
            var oldVals = _items.ToArray();
            _items.Clear();
            await Changed.Invoke(Changes.Clear<T>().Collect(oldVals, _items));
        }

        public async Task Insert(int index, T item)
        {
            var oldVals = _items.ToArray();
            _items.Insert(index, item);
            await Changed.Invoke(Changes.Add(index, item).Collect(oldVals, _items));
        }

        public async Task<T> Pop()
        {
            var oldVals = _items.ToArray();
            var index = _items.Count - 1;
            var item = _items[index];
            _items.RemoveAt(index);

            await Changed.Invoke(Changes.Remove(index, item).Collect(oldVals, _items));
            return item;
        }

        public async Task InsertRange(int index, IEnumerable<T> values)
        {
            var oldVals = _items.ToArray();
            var start = index;
            _items.InsertRange(index, values);

            var changes = values.Select((v, i) => Changes.Add(start + i, v));
            await Changed.Invoke(changes.Collect(oldVals, _items));

        }

        public async Task MoveAt(int oldIndex, int newIndex)
        {
            var oldVals = _items.ToArray();
            T item = _items[oldIndex];
            _items.RemoveAt(oldIndex);
            _items.Insert(newIndex, item);
            await Changed.Invoke(Changes.Move(oldIndex, newIndex, item).Collect(oldVals, _items));
        }

        public async Task MoveItem(T item, int newIndex)
        {
            var oldIndex = _items.IndexOf(item);
            if (oldIndex >= 0)
            {
                await MoveAt(oldIndex, newIndex);
            }
        }

        public async Task MoveWhere(Predicate<T> predicate, int newIndex)
        {
            var index = _items.FindIndex(predicate);
            if (index >= 0)
            {
                await MoveAt(index, newIndex);
            }
        }

        public async Task Remove(T item)
        {
            var oldVals = _items.ToArray();
            var index = _items.IndexOf(item);
            if (index > -1)
            {
                _items.RemoveAt(index);
                await Changed.Invoke(Changes.Remove(index, item).Collect(oldVals, _items));
            }
        }

        public async Task RemoveAt(int index)
        {
            var oldVals = _items.ToArray();
            var item = _items[index];
            _items.RemoveAt(index);
            await Changed.Invoke(Changes.Remove(index, item).Collect(oldVals, _items));
        }

        public async Task RemoveWhere(Predicate<T> predicate)
        {
            var oldVals = _items.ToArray();
            var itemsToRemove = _items
                .Select((v, i) => (index: i, value: v))
                .Where(pair => predicate(pair.value))
                .OrderByDescending(pair => pair.index)
                .Select(pair => Changes.Remove(pair.index, pair.value))
                .ToList();

            foreach (var pair in itemsToRemove)
            {
                _items.RemoveAt(pair.Index);
            }

            await Changed.Invoke(itemsToRemove.Collect(oldVals, _items));
        }

        public async Task Reset(IEnumerable<T> values)
        {
            var oldVals = _items.ToArray();
            _items = values.ToList();
            await Changed.Invoke(Changes.Reset(values).Collect(oldVals, _items));
        }

        public Task Transform(IEnumerable<T> values)
        {
            throw new NotImplementedException();
        }


        public ServiceCollectionPropertyReadonly<T> PropertyGet(ServiceBase service)
        {
            var res = _prop as ServiceCollectionPropertyReadonly<T>;

            if ((res == null) || (res.Owner != service))
            {
                res = new ServiceCollectionPropertyReadonly<T>(this, service);
                _prop = res;
            }
            return res;
        }

        public ServiceCollectionProperty<T> PropertyGetSet(ServiceBase service)
        {
            var res = _prop as ServiceCollectionProperty<T>;

            if ((res == null) || (res.Owner != service))
            {
                res = new ServiceCollectionProperty<T>(this, service);
                _prop = res;
            }
            return res;
        }

        public static implicit operator ServiceCollectionField<T>(T item)
        {
            return new ServiceCollectionField<T>(item);
        }

        public static implicit operator ServiceCollectionField<T>((T item1, T item2) data)
        {
            return new ServiceCollectionField<T>(data.item1, data.item2);
        }

        public static implicit operator ServiceCollectionField<T>((T item1, T item2, T item3) data)
        {
            return new ServiceCollectionField<T>(data.item1, data.item2, data.item3);
        }

        public static implicit operator ServiceCollectionField<T>((T item1, T item2, T item3, T item4) data)
        {
            return new ServiceCollectionField<T>(data.item1, data.item2, data.item3, data.item4);
        }

        public static implicit operator ServiceCollectionField<T>((T item1, T item2, T item3, T item4, T item5) data)
        {
            return new ServiceCollectionField<T>(data.item1, data.item2, data.item3, data.item4, data.item5);
        }

        public static implicit operator ServiceCollectionField<T>((T item1, T item2, T item3, T item4, T item5, T item6) data)
        {
            return new ServiceCollectionField<T>(data.item1, data.item2, data.item3, data.item4, data.item5, data.item6);
        }

        public static implicit operator ServiceCollectionField<T>((T item1, T item2, T item3, T item4, T item5, T item6, T item7) data)
        {
            return new ServiceCollectionField<T>(data.item1, data.item2, data.item3, data.item4, data.item5, data.item6, data.item7);
        }

        public static implicit operator ServiceCollectionField<T>((T item1, T item2, T item3, T item4, T item5, T item6, T item7, T item8) data)
        {
            return new ServiceCollectionField<T>(data.item1, data.item2, data.item3, data.item4, data.item5, data.item6, data.item7, data.item8);
        }

        public static implicit operator ServiceCollectionField<T>((T item1, T item2, T item3, T item4, T item5, T item6, T item7, T item8, T item9) data)
        {
            return new ServiceCollectionField<T>(data.item1, data.item2, data.item3, data.item4, data.item5, data.item6, data.item7, data.item8, data.item9);
        }

        public static implicit operator ServiceCollectionField<T>((T item1, T item2, T item3, T item4, T item5, T item6, T item7, T item8, T item9, T item10) data)
        {
            return new ServiceCollectionField<T>(data.item1, data.item2, data.item3, data.item4, data.item5, data.item6, data.item7, data.item8, data.item9, data.item10);
        }


    }
}
