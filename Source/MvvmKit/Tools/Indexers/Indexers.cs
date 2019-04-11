using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvvmKit
{
    public static class Indexers
    {
        public static IReadOnlyIndexer<K, T> ReadOnly<K, T>(Func<K, T> getter)
        {
            return new ReadonlyIndexer<K, T>(getter);
        }

        public static WriteableIndexer<K, T> Writeable<K, T>(Func<K, T> getter, Action<K, T> setter)
        {
            return new WriteableIndexer<K, T>(getter, setter);
        }

        public static IReadOnlyIndexer<K, T> ReadOnly<K, T>(IDictionary<K, T> dictionary)
        {
            return new ReadonlyIndexer<K, T>(k => dictionary[k]);
        }

        public static WriteableIndexer<K, T> Writeable<K, T>(IDictionary<K, T> dictionary)
        {
            return new WriteableIndexer<K, T>(k => dictionary[k], (k, t) => dictionary[k] = t);
        }


        public static IReadOnlyIndexer<K, T> ReadOnly<K, T>(this IWriteableIndexer<K, T> source)
        {
            return new ReadonlyIndexer<K, T>(source.Getter);
        }

        public static IReadOnlyIndexer<K, T> Writable<K, T>(this IReadOnlyIndexer<K, T> source, Action<K, T> setter)
        {
            return new WriteableIndexer<K, T>(source.Getter, setter);
        }

        public static IReadOnlyIndexer<K1, K2, T> And<K1, K2, T>(this IReadOnlyIndexer<K1, T> source,  Func<K2, T> getter2)
        {
            return new ReadonlyIndexer<K1, K2, T>(source.Getter, getter2);
        }

        public static IWriteableIndexer<K1, K2, T> And<K1, K2, T>(this IWriteableIndexer<K1, T> source, Func<K2, T> getter2, Action<K2, T> setter2)
        {
            return new WriteableIndexer<K1, K2, T>(source.Getter, source.Setter, getter2, setter2);
        }

        public static IReadOnlyIndexer<K1, K2, T> And<K1, K2, T>(this IReadOnlyIndexer<K1, T> source, Dictionary<K2, T> dictionary)
        {
            return new ReadonlyIndexer<K1, K2, T>(source.Getter, k => dictionary[k]);
        }

        public static IWriteableIndexer<K1, K2, T> And<K1, K2, T>(this IWriteableIndexer<K1, T> source, IDictionary<K2, T> dictionary)
        {
            return new WriteableIndexer<K1, K2, T>(source.Getter, source.Setter,  k => dictionary[k], (k, t) => dictionary[k] = t);
        }

        public static IReadOnlyIndexer<K1, K2, T> ReadOnly<K1, K2, T>(this IWriteableIndexer<K1, K2, T> source)
        {
            return new ReadonlyIndexer<K1, K2, T>(source.Getter, source.Getter2);
        }

        public static IWriteableIndexer<K1, K2, T> Writeable<K1, K2, T>(this IReadOnlyIndexer<K1, K2, T> source, Action<K1, T> setter1, Action<K2, T> setter2)
        {
            return new WriteableIndexer<K1, K2, T>(source.Getter, setter1, source.Getter2, setter2);
        }

        public static IReadOnlyIndexer<K1, K2, K3, T> And<K1, K2, K3, T>(this IReadOnlyIndexer<K1, K2, T> source, Func<K3, T> getter3)
        {
            return new ReadonlyIndexer<K1, K2, K3, T>(source.Getter, source.Getter2, getter3);
        }

        public static IWriteableIndexer<K1, K2, K3, T> And<K1, K2, K3, T>(this IWriteableIndexer<K1, K2, T> source, Func<K3, T> getter3, Action<K3, T> setter3)
        {
            return new WriteableIndexer<K1, K2, K3, T>(source.Getter, source.Setter, source.Getter2, source.Setter2, getter3, setter3);
        }

        public static IReadOnlyIndexer<K1, K2, K3, T> And<K1, K2, K3, T>(this IReadOnlyIndexer<K1, K2, T> source, IDictionary<K3, T> dictionary)
        {
            return new ReadonlyIndexer<K1, K2, K3, T>(source.Getter, source.Getter2, k => dictionary[k]);
        }

        public static IWriteableIndexer<K1, K2, K3, T> And<K1, K2, K3, T>(this IWriteableIndexer<K1, K2, T> source, IDictionary<K3, T> dictionary)
        {
            return new WriteableIndexer<K1, K2, K3, T>(source.Getter, source.Setter, source.Getter2, source.Setter2, k => dictionary[k], (k, t) => dictionary[k] = t);
        }

        public static IReadOnlyIndexer<K1, K2, K3, T> ReadOnly<K1, K2, K3, T>(this IWriteableIndexer<K1, K2, K3, T> source)
        {
            return new ReadonlyIndexer<K1, K2, K3, T>(source.Getter, source.Getter2, source.Getter3);
        }

        public static IWriteableIndexer<K1, K2, K3, T> Writeable<K1, K2, K3, T>(this IReadOnlyIndexer<K1, K2, K3, T> source, Action<K1, T> setter1, Action<K2, T> setter2, Action<K3, T> setter3)
        {
            return new WriteableIndexer<K1, K2, K3, T>(source.Getter, setter1, source.Getter2, setter2, source.Getter3, setter3);
        }

        public static IReadOnlyIndexer<K1, K2, K3, K4, T> And<K1, K2, K3, K4, T>(this IReadOnlyIndexer<K1, K2, K3, T> source, Func<K4, T> getter4)
        {
            return new ReadonlyIndexer<K1, K2, K3, K4, T>(source.Getter, source.Getter2, source.Getter3, getter4);
        }

        public static IWriteableIndexer<K1, K2, K3, K4, T> And<K1, K2, K3, K4, T>(this IWriteableIndexer<K1, K2, K3, T> source, Func<K4, T> getter4, Action<K4, T> setter4)
        {
            return new WriteableIndexer<K1, K2, K3, K4, T>(source.Getter, source.Setter, source.Getter2, source.Setter2, source.Getter3, source.Setter3, getter4, setter4);
        }

        public static IReadOnlyIndexer<K1, K2, K3, K4, T> And<K1, K2, K3, K4, T>(this IReadOnlyIndexer<K1, K2, K3, T> source, IDictionary<K4, T> dictionary)
        {
            return new ReadonlyIndexer<K1, K2, K3, K4, T>(source.Getter, source.Getter2, source.Getter3, k => dictionary[k]);
        }

        public static IWriteableIndexer<K1, K2, K3, K4, T> And<K1, K2, K3, K4, T>(this IWriteableIndexer<K1, K2, K3, T> source, IDictionary<K4, T> dictionary)
        {
            return new WriteableIndexer<K1, K2, K3, K4, T>(source.Getter, source.Setter, source.Getter2, source.Setter2, source.Getter3, source.Setter3, k => dictionary[k], (k, t) => dictionary[k] = t);
        }

        public static IReadOnlyIndexer<K1, K2, K3, K4, T> ReadOnly<K1, K2, K3, K4, T>(this IWriteableIndexer<K1, K2, K3, K4, T> source)
        {
            return new ReadonlyIndexer<K1, K2, K3, K4, T>(source.Getter, source.Getter2, source.Getter3, source.Getter4);
        }

        public static IWriteableIndexer<K1, K2, K3, K4, T> Writeable<K1, K2, K3, K4, T>(this IReadOnlyIndexer<K1, K2, K3, K4, T> source, Action<K1, T> setter1, Action<K2, T> setter2, Action<K3, T> setter3, Action<K4, T> setter4)
        {
            return new WriteableIndexer<K1, K2, K3, K4, T>(source.Getter, setter1, source.Getter2, setter2, source.Getter3, setter3, source.Getter4, setter4);
        }


    }
}