using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface ICollectionChangeMove : ICollectionChange
    {
        int FromIndex { get; }

        int ToIndex { get; }

        object Item { get; }
    }

    public interface ICollectionChangeMove<T> : ICollectionChangeMove, ICollectionChange<T>
    {
        new T Item { get; }
    }
}
