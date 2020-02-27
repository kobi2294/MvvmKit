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
        public static ImmutableList<T> UpsertWhere<T>(this ImmutableList<T> source, Func<T, bool> predicate, T item)
        {
            var index = source.IndexOf(predicate);
            if (index < 0) return source.Add(item);

            return source.SetItem(index, item);
        }

        public static ImmutableList<T> Upsert<T, K>(this ImmutableList<T> source, Func<T, K> trackBy, T item)
        {
            var key = trackBy(item);

            return source.UpsertWhere(t => Equals(trackBy(t), key), item);
        }

        public static ImmutableList<T> Upsert<T>(this ImmutableList<T> source, T item)
        {
            return source.Upsert(t => t, item);
        }

        public static VersionedList<T> UpsertWhere<T>(this VersionedList<T> source, Func<T, bool> predicate, T item)
        {
            var index = source.IndexOf(predicate);
            if (index < 0) return source.Add(item);

            return source.SetItem(index, item);
        }

        public static VersionedList<T> Upsert<T, K>(this VersionedList<T> source, Func<T, K> trackBy, T item)
        {
            var key = trackBy(item);

            return source.UpsertWhere(t => Equals(trackBy(t), key), item);
        }

        public static VersionedList<T> Upsert<T>(this VersionedList<T> source, T item)
        {
            return source.Upsert(t => t, item);
        }

    }
}
