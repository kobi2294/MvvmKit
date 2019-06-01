using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface ICollectionChangeAdd : ICollectionChange
    {
        int Index { get; }

        object Item { get; }
    }

    public interface ICollectionChangeAdd<T>: ICollectionChangeAdd, ICollectionChange<T>
    {
        new T Item { get; }
    }
}
