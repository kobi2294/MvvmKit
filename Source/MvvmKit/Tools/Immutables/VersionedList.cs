using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public sealed class VersionedList<T>
    {
        public ImmutableList<T> Items { get; }
        public int Version { get; }
        public ImmutableList<IListChange<T>> Changes { get; }

        private VersionedList(ImmutableList<T> items, int version, ImmutableList<IListChange<T>> changes)
        {
            Items = items;
            Version = version;
            Changes = changes;
        }

        private VersionedList(ImmutableList<T> items, int version)
            : this(items, version, ImmutableList<IListChange<T>>.Empty)
        {
            Changes = Changes.Add(ListChange.Reset(Version - 1, Items));
        }

        private VersionedList(ImmutableList<T> items)
            :this(items, 0, ImmutableList<IListChange<T>>.Empty)
        {
        }


        public static VersionedList<T> Empty { get; } = new VersionedList<T>(ImmutableList<T>.Empty);

        public T this[int index] => Items[index];
        public bool IsEmpty => Items.IsEmpty;
        public int Count => Items.Count;
        public int BinarySearch(T item) => Items.BinarySearch(item);
        public int BinarySearch(T item, IComparer<T> comparer) => Items.BinarySearch(item, comparer);
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer) 
            => Items.BinarySearch(index, count, item, comparer);
        public bool Contains(T value) => Items.Contains(value);
        public void CopyTo(T[] array) => Items.CopyTo(array);
        public void CopyTo(int index, T[] array, int arrayIndex, int count) 
            => Items.CopyTo(index, array, arrayIndex, count);
        public void CopyTo(T[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);
        public bool Exists(Predicate<T> match) => Items.Exists(match);
        public T Find(Predicate<T> match) => Items.Find(match);
        public int FindIndex(int startIndex, int count, Predicate<T> match) 
            => Items.FindIndex(startIndex, count, match);
        public int FindIndex(int startIndex, Predicate<T> match) 
            => Items.FindIndex(startIndex, match);
        public int FindIndex(Predicate<T> match) => Items.FindIndex(match);
        public T FindLast(Predicate<T> match) => Items.FindLast(match);
        public int FindLastIndex(int startIndex, Predicate<T> match) 
            => Items.FindLastIndex(startIndex, match);
        public int FindLastIndex(Predicate<T> match) => Items.FindLastIndex(match);
        public int FindLastIndex(int startIndex, int count, Predicate<T> match) 
            => Items.FindLastIndex(startIndex, count, match);
        public void ForEach(Action<T> action) => Items.ForEach(action);
        public IEnumerator GetEnumerator() => Items.GetEnumerator();
        public int IndexOf(T value) => Items.IndexOf(value);
        public int IndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer) 
            => Items.IndexOf(item, index, count, equalityComparer);
        public ref readonly T ItemRef(int index) => ref Items.ItemRef(index);
        public int LastIndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer) 
            => Items.LastIndexOf(item, index, count, equalityComparer);

        public VersionedList<T> Add(T item)
        {
            return new VersionedList<T>(
                Items.Add(item),
                Version + 1,
                Changes.Add(ListChange.Added(Version, Items.Count, item)));
        }

        public VersionedList<T> AddRange(IEnumerable<T> items)
        {
            var itemsToAdd = items.ToArray();
            var changes = Enumerable.Range(0, itemsToAdd.Length)
                .Select(i => ListChange.Added(Version + i, Items.Count + i, itemsToAdd[i]));               

            return new VersionedList<T>(
                Items.AddRange(items), 
                Version + itemsToAdd.Length, 
                Changes.AddRange(changes)
                );
        }

        public VersionedList<T> Clear()
        {
            return new VersionedList<T>(
                ImmutableList<T>.Empty, 
                Version + 1);
        }

        public VersionedList<TOutput> ConvertAll<TOutput>(Func<T, TOutput> converter)
        {
            var items = Items.ConvertAll(converter);
            return new VersionedList<TOutput>(items);
        }

        private VersionedList<T> _removeWhere(Func<T, int, bool> predicate)
        {
            var pairs = Items
                .Select((T item, int i) => new { Index = i, Item = item })
                .ToArray();

            var pairsToRemove = pairs
                .Where(pair => predicate(pair.Item, pair.Index))
                .OrderByDescending(pair => pair.Index)
                .ToArray();

            var remaining = pairs
                .Where(pair => !predicate(pair.Item, pair.Index))
                .Select(pair => pair.Item);

            var changes = Enumerable.Range(0, pairsToRemove.Length)
                .Select(i => ListChange.Removed(Version + i, pairsToRemove[i].Index, pairsToRemove[i].Item));

            return new VersionedList<T>(
                ImmutableList.CreateRange(remaining),
                Version + pairsToRemove.Length,
                Changes.AddRange(changes)
                );
        }

        public VersionedList<T> FindAll(Predicate<T> match)
        {
            return _removeWhere((T item, int index) => !match(item));
        }

        public VersionedList<T> GetRange(int index, int count)
        {
            return _removeWhere((T item, int i) => (i < index) && (i >= index + count));
        }

        public VersionedList<T> Insert(int index, T item)
        {
            return new VersionedList<T>(
                Items.Insert(index, item),
                Version + 1,
                Changes.Add(ListChange.Added(Version, index, item)));
        }

        public VersionedList<T> InsertRange(int index, IEnumerable<T> items)
        {
            var itemsToAdd = items.ToArray();
            var changes = Enumerable.Range(0, itemsToAdd.Length)
                .Select(i => ListChange.Added(Version + i, index + i, itemsToAdd[i]));
            return new VersionedList<T>(
                Items.InsertRange(index, items),
                Version + itemsToAdd.Length,
                Changes.AddRange(changes)
                );
        }

        public VersionedList<T> Remove(T value)
        {
            var index = Items.IndexOf(value);
            return _removeWhere((item, i) => i == index);
        }

        public VersionedList<T> Remove(T value, IEqualityComparer<T> equalityComparer)
        {
            var index = Items.IndexOf(value, equalityComparer);
            return _removeWhere((item, i) => i == index);
        }

        public VersionedList<T> RemoveAll(Predicate<T> match)
        {
            return _removeWhere((item, i) => match(item));
        }

        public VersionedList<T> RemoveAt(int index)
        {
            return _removeWhere((item, i) => i == index);
        }

        public VersionedList<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            var toRemove = items.ToHashSet();
            return _removeWhere((item, i) => toRemove.Contains(item, equalityComparer));
        }

        public VersionedList<T> RemoveRange(IEnumerable<T> items)
        {
            var toRemove = items.ToHashSet();
            return _removeWhere((item, i) => toRemove.Contains(item));
        }

        public VersionedList<T> RemoveRange(int index, int count)
        {
            return _removeWhere((item, i) => (i >= index) && (i < index + count));
        }

        public VersionedList<T> Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer)
        {
            var index = Items.IndexOf(oldValue, equalityComparer);
            return new VersionedList<T>(
                Items.Replace(oldValue, newValue, equalityComparer), 
                Version + 1, 
                Changes.Add(ListChange.Replaced(Version, index, oldValue, newValue))
                );
        }

        public VersionedList<T> Replace(T oldValue, T newValue)
        {
            var index = Items.IndexOf(oldValue);
            return new VersionedList<T>(
                Items.Replace(oldValue, newValue),
                Version + 1,
                Changes.Add(ListChange.Replaced(Version, index, oldValue, newValue))
                );
        }

        public VersionedList<T> Reset(IEnumerable<T> items)
        {
            return new VersionedList<T>(ImmutableList.CreateRange(items), Version + 1);
        }

        public VersionedList<T> Reverse(int index, int count)
        {
            var start = index;
            var finish = index + count - 1;

            var moves = Enumerable.Range(0, count)
                .Select(i => ListChange.Moved(Version + i, start, finish - i, Items[start + i]));

            return new VersionedList<T>(Items.Reverse(index, count), Version + count, Changes.AddRange(moves));
        }

        public VersionedList<T> Reverse()
        {
            return Reverse(0, Items.Count);
        }

        public VersionedList<T> SetItem(int index, T newItem)
        {
            var oldItem = Items[index];
            return new VersionedList<T>(
                Items.SetItem(index, newItem), 
                Version + 1, 
                Changes.Add(ListChange.Replaced(Version, index, oldItem, newItem))
                );
        }

        public VersionedList<T> Sort(int index, int count, IComparer<T> comparer)
        {
            return new VersionedList<T>(Items.Sort(index, count, comparer), Version + 1);
        }

        public VersionedList<T> Sort(IComparer<T> comparer)
        {
            return new VersionedList<T>(Items.Sort(comparer), Version + 1);
        }

        public VersionedList<T> Sort(Comparison<T> comparison)
        {
            return new VersionedList<T>(Items.Sort(comparison), Version + 1);
        }

        public VersionedList<T> Sort()
        {
            return new VersionedList<T>(Items.Sort(), Version + 1);
        }
    }
}
