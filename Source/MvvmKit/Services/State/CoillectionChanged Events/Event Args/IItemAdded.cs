using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public interface IItemAdded : IChange
    {
        int Index { get; }

        object Item { get; }
    }

    public interface IItemAdded<T>: IItemAdded, IChange<T>
    {
        new T Item { get; }
    }
}
