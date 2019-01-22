using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static partial class Disposables
    {
        public static IDisposableWithData<T> Call<T>(Action<IDisposableWithData<T>> action, T data)
        {
            return new CallbackWithDataAndSelf<T>(action, data);
        }

        private class CallbackWithDataAndSelf<T> : BaseDisposableWithData<T>
        {
            private Action<IDisposableWithData<T>> _callback;

            public CallbackWithDataAndSelf(Action<IDisposableWithData<T>> callback, T data)
            {
                Data = data;
                _callback = callback;
            }

            protected override void OnDisposed()
            {
                base.OnDisposed();
                _callback(this);
            }
        }

    }
}
