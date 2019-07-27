using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public interface IReset : IChange
    {
        IEnumerable<object> Items { get; }
    }

    public interface IReset<T> : IReset, IChange<T>
    {
        new IEnumerable<T> Items { get; }
    }
}
