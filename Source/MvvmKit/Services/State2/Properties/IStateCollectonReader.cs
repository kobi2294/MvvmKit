using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface IStateCollectionReader<T>
    {
        AsyncEvent<CollectionChanges<T>> Changed { get; }

        Task<IReadOnlyList<T>> Get();

        Task<TRes> Select<TRes>(Func<IStateList<T>, TRes> selector);
    }
}
