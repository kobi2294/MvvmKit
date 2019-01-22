using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface IDisposableWithData : IDisposable
    {
        object Data { get; }
    }

    public interface IDisposableWithData<T> : IDisposableWithData
    {
        new T Data { get; }
    }
}
