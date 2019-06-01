using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface ICollectionChangeRemove : ICollectionChange
    {
        int Index { get; }

        object Item { get; }
    }

    public interface ICollectionChangeRemove<T> : ICollectionChangeRemove, ICollectionChange<T>
    {
        new T Item { get; }
    }
}
