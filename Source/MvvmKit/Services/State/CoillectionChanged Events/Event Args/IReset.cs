using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public interface IReset : IChange
    {
    }

    public interface IReset<T>: IReset, IChange<T>
    {
    }
}
