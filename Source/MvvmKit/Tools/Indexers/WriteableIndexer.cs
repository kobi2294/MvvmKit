using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvvmKit
{
    public class WriteableIndexer<K, T> : IWriteableIndexer<K, T>
    {
        public Func<K, T> Getter { get; }
        public Action<K, T> Setter { get; }

        public T this[K key] {
            get => Getter(key);
            set => Setter(key, value);
        }

        T IReadOnlyIndexer<K, T>.this[K key] => this[key];

        public WriteableIndexer(Func<K, T> getter, Action<K, T> setter) 
        {
            Getter = getter;
            Setter = setter;
        }
    }

    public class WriteableIndexer<K1, K2, T> : WriteableIndexer<K1, T>, IWriteableIndexer<K1, K2, T>
    {
        public Func<K2, T> Getter2 { get; }
        public Action<K2, T> Setter2 { get; }

        public T this[K2 key]
        {
            get => Getter2(key);
            set => Setter2(key, value);
        }

        T IReadOnlyIndexer<K1, K2, T>.this[K2 key] => this[key];

        public WriteableIndexer(Func<K1, T> getter1, Action<K1, T> setter1, Func<K2, T> getter2, Action<K2, T> setter2)
            :base(getter1, setter1)
        {
            Getter2 = getter2;
            Setter2 = setter2;
        }
    }

    public class WriteableIndexer<K1, K2, K3, T> : WriteableIndexer<K1, K2, T>, IWriteableIndexer<K1, K2, K3, T>
    {
        public Func<K3, T> Getter3 { get; }
        public Action<K3, T> Setter3 { get; }

        public T this[K3 key]
        {
            get => Getter3(key);
            set => Setter3(key, value);
        }

        T IReadOnlyIndexer<K1, K2, K3, T>.this[K3 key] => this[key];

        public WriteableIndexer(Func<K1, T> getter1, Action<K1, T> setter1, 
                                Func<K2, T> getter2, Action<K2, T> setter2, 
                                Func<K3, T> getter3, Action<K3, T> setter3)
            : base(getter1, setter1, getter2, setter2)
        {
            Getter3 = getter3;
            Setter3 = setter3;
        }
    }

    public class WriteableIndexer<K1, K2, K3, K4, T> : WriteableIndexer<K1, K2, K3, T>, IWriteableIndexer<K1, K2, K3, K4, T>
    {
        public Func<K4, T> Getter4 { get; }
        public Action<K4, T> Setter4 { get; }

        public T this[K4 key]
        {
            get => Getter4(key);
            set => Setter4(key, value);
        }

        T IReadOnlyIndexer<K1, K2, K3, K4, T>.this[K4 key] => this[key];

        public WriteableIndexer(Func<K1, T> getter1, Action<K1, T> setter1,
                                Func<K2, T> getter2, Action<K2, T> setter2,
                                Func<K3, T> getter3, Action<K3, T> setter3,
                                Func<K4, T> getter4, Action<K4, T> setter4)
            : base(getter1, setter1, getter2, setter2, getter3, setter3)
        {
            Getter4 = getter4;
            Setter4 = setter4;
        }
    }
}