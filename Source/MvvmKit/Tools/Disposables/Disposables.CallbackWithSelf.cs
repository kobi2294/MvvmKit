using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static partial class Disposables
    {
        public static IDisposable Call(Action<IDisposable> callback)
        {
            return new CallbackWithSelf(callback);
        }

        private class CallbackWithSelf : BaseDisposable
        {
            private Action<IDisposable> _callback;

            public CallbackWithSelf(Action<IDisposable> callback)
            {
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
