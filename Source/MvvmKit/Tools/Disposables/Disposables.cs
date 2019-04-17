using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static partial class Disposables
    {
        public static BaseDisposableWithData<T> WithData<T>(T data)
        {
            return new BaseDisposableWithData<T>(data);
        }
    }
}
