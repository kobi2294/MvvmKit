using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal interface IListModifier<T>
        where T: class, IImmutable
    {
        ImmutableList<T> Modify(ImmutableList<T> source);
    }
}
