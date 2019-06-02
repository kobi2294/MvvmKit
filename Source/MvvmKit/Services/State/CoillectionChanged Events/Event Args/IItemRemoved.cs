using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public interface IItemRemoved : IChange
    {
        int Index { get; }

        object Item { get; }
    }

    public interface IItemRemoved<T> : IItemRemoved, IChange<T>
    {
        new T Item { get; }
    }
}
