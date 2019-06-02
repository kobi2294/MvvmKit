using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public interface IItemReplaced : IChange
    {
        int Index { get; }

        object FromItem { get; }

        object ToItem { get; }
    }

    public interface IItemReplaced<T> : IItemReplaced, IChange<T>
    {
        new T FromItem { get; }

        new T ToItem { get; }
    }
}
