using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public interface IItemMoved : IChange
    {
        int FromIndex { get; }

        int ToIndex { get; }

        object Item { get; }
    }

    public interface IItemMoved<T> : IItemMoved, IChange<T>
    {
        new T Item { get; }
    }
}
