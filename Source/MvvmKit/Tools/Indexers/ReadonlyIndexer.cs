using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvvmKit
{
    public class ReadonlyIndexer<K, T> : IReadOnlyIndexer<K, T>
    {
        public Func<K, T> Getter { get; }

        public T this[K key] => Getter(key);

        internal ReadonlyIndexer(Func<K, T> getter)
        {
            Getter = getter;
        }
    }

    public class ReadonlyIndexer<K1, K2, T> : ReadonlyIndexer<K1, T>, IReadOnlyIndexer<K1, K2, T>
    {
        public Func<K2, T> Getter2 { get; }

        public T this[K2 key] => Getter2(key);

        internal ReadonlyIndexer(Func<K1, T> getter1, Func<K2, T> getter2)
            :base(getter1)
        {
            Getter2 = getter2;
        }
    }

    public class ReadonlyIndexer<K1, K2, K3, T> : ReadonlyIndexer<K1, K2, T>, IReadOnlyIndexer<K1, K2, K3, T>
    {
        public Func<K3, T> Getter3 { get; }

        public T this[K3 key] => Getter3(key);

        internal ReadonlyIndexer(Func<K1, T> getter1, Func<K2, T> getter2, Func<K3, T> getter3)
            : base(getter1, getter2)
        {
            Getter3 = getter3;
        }
    }

    public class ReadonlyIndexer<K1, K2, K3, K4, T> : ReadonlyIndexer<K1, K2, K3, T>, IReadOnlyIndexer<K1, K2, K3, K4, T>
    {
        public Func<K4, T> Getter4 { get; }

        public T this[K4 key] => Getter4(key);

        internal ReadonlyIndexer(Func<K1, T> getter1, Func<K2, T> getter2, Func<K3, T> getter3, Func<K4, T> getter4)
            : base(getter1, getter2, getter3)
        {
            Getter4 = getter4;
        }
    }

}