using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public interface ICleared : IChange
    {
    }

    public interface ICleared<T> : ICleared, IChange<T>
    {
    }
}
