using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class ImmutableExtensions
    {
        public static ImmutableList<T> UpsertWhere<T>(this ImmutableList<T> source, T item, Func<T, bool> predicate)
        {
            var index = source.IndexOf(predicate);
            if (index < 0) return source.Add(item);

            return source.SetItem(index, item);
        }

        public static ImmutableList<T> Upsert<T, K>(this ImmutableList<T> source, T item, Func<T, K> keySelector)
        {
            var key = keySelector(item);

            return source.UpsertWhere(item, t => Equals(keySelector(t), key));
        }

        public static ImmutableList<T> Upsert<T>(this ImmutableList<T> source, T item)
        {
            return source.Upsert(item, t => t);
        }

        public static VersionedList<T> UpsertWhere<T>(this VersionedList<T> source, T item, Func<T, bool> predicate)
        {
            var index = source.IndexOf(predicate);
            if (index < 0) return source.Add(item);

            return source.SetItem(index, item);
        }

        public static VersionedList<T> Upsert<T, K>(this VersionedList<T> source, T item, Func<T, K> keySelector)
        {
            var key = keySelector(item);

            return source.UpsertWhere(item, t => Equals(keySelector(t), key));
        }

        public static VersionedList<T> Upsert<T>(this VersionedList<T> source, T item)
        {
            return source.Upsert(item, t => t);
        }

    }
}
