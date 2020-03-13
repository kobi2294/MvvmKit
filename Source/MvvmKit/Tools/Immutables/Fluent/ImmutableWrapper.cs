using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class ImmutableWrapper<T>
        where T: class, IImmutable
    {
        public abstract T Go();

        public static implicit operator T(ImmutableWrapper<T> value)
        {
            return value.Go();
        }
    }
}
