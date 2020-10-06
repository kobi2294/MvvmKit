using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal interface IInstanceModifier<T>
        where T: class, IImmutable
    {
        T Modify(T source);
    }
}
