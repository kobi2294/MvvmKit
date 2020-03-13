using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class DiffAlgorithm
    {
        /// <summary>
        /// Distinctify returns an enumerable in the same order as the source, while adding a counter next to each item. The counter describes
        /// the number of occurences that already incured for the same element key. The trackBy function creates comparable keys for each item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<(K key, int counter, T item)> Distinctify<T, K>(this IEnumerable<T> source, Func<T, K> trackBy) 
        {
            var counters = new Dictionary<K, int>();

            foreach (var item in source)
            {
                var key = trackBy(item);
                var counter = counters.GetOrCreate(key, () => 0);

                yield return (key, counter, item);
                counters[key]++;
            }
        }

        public static IEnumerable<(int counter, T item)> Distinctify<T>(this IEnumerable<T> source)
        {
            return source.Distinctify(x => x)
                .Select(record => (record.counter, record.item));
        }

        public static DiffResults<T> Diff<T, K>(this IEnumerable<T> source, IEnumerable<T> target, Func<T, K> trackBy)
        {
            var src = source.Distinctify(trackBy).ToArray();
            var trg = target.Distinctify(trackBy).ToArray();

            // now we have source and target lists with certain distinct items
            var srcIndices = src
                .Enumerated()
                .Select(record => (record.index, record.item.item, record.item.key, record.item.counter))
                .ToDictionary(record => (record.key, record.counter), record => (record.item, record.index));

            var trgIndices = trg
                .Enumerated()
                .Select(record => (record.index, record.item.item, record.item.key, record.item.counter))
                .ToDictionary(record => (record.key, record.counter), record => (record.item, record.index));

            var srcKeys = srcIndices.Keys.ToArray();

            var trgKeys = trgIndices.Keys.ToArray();

            var added = trgKeys
                .Except(srcKeys)
                .Select(key => (to: trgIndices[key].index, item: trgIndices[key].item))
                .OrderBy(entry => entry.to)
                .ToImmutableList();

            var removed = srcKeys
                .Except(trgKeys)
                .Select(key => (from: srcIndices[key].index, item: srcIndices[key].item))
                .OrderByDescending(entry => entry.from)
                .ToImmutableList();

            var srcCommon = srcKeys
                .Intersect(trgKeys)
                .ToList();

            var srcCommonMap = srcCommon.ToDictionary(entry => (entry.key, entry.counter));

            var trgCommon = trgKeys
                .Intersect(srcKeys)
                .ToList();

            var moved = srcCommon
                .Permute(trgCommon)
                .ToImmutableList();

            var modified = srcCommon
                .Select(key => (at: trgIndices[key].index, old: srcIndices[key].item, @new: trgIndices[key].item))
                .Where(entry => !Equals(entry.old, entry.@new))
                .ToImmutableList();

            return new DiffResults<T>(
                removed: removed,
                moved: moved,
                added: added,
                modified: modified);
        }

        public static DiffResults<T> Diff<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            return source.Diff(target, x => x);
        }

        public static void ApplyDiff<T, K>(this ObservableCollection<K> source, 
            DiffResults<T> diff,
            Func<int, T, K> onAdd,
            Action<int, T, K> onRemove = null, 
            Action<int, int, K> onMove = null, 
            Action<int, T, T, K> onModify = null)
        {
            foreach (var remove in diff.Removed)
            {
                onRemove?.Invoke(remove.from, remove.item, source[remove.from]);
                source.RemoveAt(remove.from);
            }

            foreach (var move in diff.Moved)
            {
                source.Move(move.from, move.to);
                onMove?.Invoke(move.from, move.to, source[move.to]);
            }

            foreach (var add in diff.Added)
            {
                var newItem = onAdd(add.to, add.item);
                source.Insert(add.to, newItem);
            }

            foreach (var modify in diff.Modified)
            {
                onModify?.Invoke(modify.at, modify.old, modify.@new, source[modify.at]);
            }
        }

        public static void ApplyDiff<T, K>(this IList<K> source,
            DiffResults<T> diff,
            Func<int, T, K> onAdd,
            Action<int, T, K> onRemove = null,
            Action<int, int, K> onMove = null,
            Action<int, T, T, K> onModify = null)
        {
            foreach (var remove in diff.Removed)
            {
                onRemove?.Invoke(remove.from, remove.item, source[remove.from]);
                source.RemoveAt(remove.from);
            }

            foreach (var move in diff.Moved)
            {
                var item = source[move.from];
                source.RemoveAt(move.from);
                source.Insert(move.to, item);
                onMove?.Invoke(move.from, move.to, source[move.to]);
            }

            foreach (var add in diff.Added)
            {
                var newItem = onAdd(add.to, add.item);
                source.Insert(add.to, newItem);
            }

            foreach (var modify in diff.Modified)
            {
                onModify?.Invoke(modify.at, modify.old, modify.@new, source[modify.at]);
            }
        }

        public static void ApplyDiff<T>(this ObservableCollection<T> source, DiffResults<T> diff)
        {
            foreach (var remove in diff.Removed)
            {
                source.RemoveAt(remove.from);
            }

            foreach (var move in diff.Moved)
            {
                source.Move(move.from, move.to);
            }

            foreach (var add in diff.Added)
            {
                source.Insert(add.to, add.item);
            }

            foreach (var modify in diff.Modified)
            {
                source[modify.at] = modify.@new;
            }
        }


        public static void ApplyDiff<T>(this IList<T> source, DiffResults<T> diff)
        {
            foreach (var remove in diff.Removed)
            {
                source.RemoveAt(remove.from);
            }

            foreach (var move in diff.Moved)
            {
                var item = source[move.from];
                source.RemoveAt(move.from);
                source.Insert(move.to, item);
            }

            foreach (var add in diff.Added)
            {
                source.Insert(add.to, add.item);
            }

            foreach (var modify in diff.Modified)
            {
                source[modify.at] = modify.@new;
            }
        }


    }
}
