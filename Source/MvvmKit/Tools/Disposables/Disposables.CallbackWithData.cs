using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static partial class Disposables
    {
        public static IDisposableWithData<T> Call<T>(Action<T> action, T data)
        {
            return new CallbackWithData<T>(action, data);
        }

        private class CallbackWithData<T> : BaseDisposableWithData<T>
        {
            private Action<T> _callback;

            public CallbackWithData(Action<T> callback, T data)
            {
                Data = data;
                _callback = callback;
            }

            protected override void OnDisposed()
            {
                base.OnDisposed();
                _callback(Data);
            }
        }

    }
}
