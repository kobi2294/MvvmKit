using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface IWriteableIndexer<K, T> : IReadOnlyIndexer<K, T>
    {
        new T this[K key] { get; set; }
        Action<K, T> Setter { get; }
    }

    public interface IWriteableIndexer<K1, K2, T> : IReadOnlyIndexer<K1, K2, T>, IWriteableIndexer<K1, T>
    {
        new T this[K2 key] { get; set; }
        Action<K2, T> Setter2 { get; }
    }

    public interface IWriteableIndexer<K1, K2, K3, T> : IReadOnlyIndexer<K1, K2, K3, T>, IWriteableIndexer<K1, K2, T>
    {
        new T this[K3 key] { get; set; }
        Action<K3, T> Setter3 { get; }
    }

    public interface IWriteableIndexer<K1, K2, K3, K4, T> : IReadOnlyIndexer<K1, K2, K3, K4, T>, IWriteableIndexer<K1, K2, K3, T>
    {
        new T this[K4 key] { get; set; }
        Action<K4, T> Setter4 { get; }
    }

}
