using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public interface IChange
    {
        ChangeType ChangeType { get; }

        IReadOnlyList<object> CurrentItems { get; }
    }

    public interface IChange<T>: IChange
    {
        new IReadOnlyList<T> CurrentItems { get; }
    }
}
