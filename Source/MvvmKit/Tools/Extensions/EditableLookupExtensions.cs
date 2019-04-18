using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class EditableLookupExtensions
    {
        public static EditableGrouping<K, T> ToEditableGrouping<K, T>(this IGrouping<K, T> source)
        {
            var res = new EditableGrouping<K, T>(source.Key, source);
            return res;
        }

        public static EditableGrouping<K, T> ToEditableGrouping<K, T>(this IEnumerable<T> source, K key)
        {
            var res = new EditableGrouping<K, T>(key, source);
            return res;
        }

        public static EditableLookup<K, T> ToEditableLookup<K, T>(this IEnumerable<IGrouping<K, T>> source)
        {
            var res = new EditableLookup<K, T>();

            foreach (var grouping in source)
            {
                res.Reset(grouping.Key, grouping);
            }

            return res;
        }

        public static EditableLookup<K, T> ToEditableLookup<K, T>(this IEnumerable<(K, T)> source)
        {
            var groupings = source.GroupBy(pair => pair.Item1, pair => pair.Item2);
            return groupings.ToEditableLookup();
        }

        public static EditableLookup<K, T> ToEditableLookup<K, T>(this IEnumerable<T> source, Func<T, K> keySelector)
        {
            var groupings = source.GroupBy(keySelector);
            return groupings.ToEditableLookup();
        }

        public static EditableLookup<K, T> ToEditableLookup<K, T, S>(this IEnumerable<S> source, Func<S, K> keySelector, Func<S, T> valueSelector)
        {
            var groupings = source.GroupBy(keySelector, valueSelector);
            return groupings.ToEditableLookup();
        }

        public static EditableLookup<K, T> ToEditableLookup<K, T>(this IDictionary<K, IEnumerable<T>> source)
        {
            var res = new EditableLookup<K, T>();

            foreach (var k in source.Keys)
            {
                res.Reset(k, source[k]);
            }

            return res;
        }

        public static EditableLookup<K, T> ToEditableLookup<K, T>(this IDictionary<K, List<T>> source)
        {
            var res = new EditableLookup<K, T>();

            foreach (var k in source.Keys)
            {
                res.Reset(k, source[k]);
            }

            return res;
        }


    }
}
