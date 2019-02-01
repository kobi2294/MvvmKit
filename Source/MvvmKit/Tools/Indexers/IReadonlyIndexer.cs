using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface IReadOnlyIndexer<K, T>
    {
        T this[K key] { get; }
        Func<K, T> Getter { get; }
    }

    public interface IReadOnlyIndexer<K1, K2, T> : IReadOnlyIndexer<K1, T>
    {
        T this[K2 key] { get; }
        Func<K2, T> Getter2 { get; }
    }

    public interface IReadOnlyIndexer<K1, K2, K3, T> : IReadOnlyIndexer<K1, K2, T>
    {
        T this[K3 key] { get; }
        Func<K3, T> Getter3 { get; }
    }

    public interface IReadOnlyIndexer<K1, K2, K3, K4, T> : IReadOnlyIndexer<K1, K2, K3, T>
    {
        T this[K4 key] { get; }
        Func<K4, T> Getter4 { get; }
    }
}
