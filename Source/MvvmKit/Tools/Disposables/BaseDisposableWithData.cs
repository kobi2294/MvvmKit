using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class BaseDisposableWithData<T> : BaseDisposable, IDisposableWithData<T>
    {
        public T Data { get; protected set; }

        object IDisposableWithData.Data { get { return this.Data; } }

        public BaseDisposableWithData(T data)
        {
            Data = data;
        }

        public BaseDisposableWithData()
        {
        }
    }
}
