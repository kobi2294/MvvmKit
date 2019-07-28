using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface IStateProperty<T> : IStatePropertyReader<T>
    {
        Task Set(T value);

    }
}
