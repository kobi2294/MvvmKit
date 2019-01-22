using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static partial class Disposables
    {
        public static IDisposable Call(Action action)
        {
            return new Callback(action);
        }

        private class Callback : BaseDisposable
        {
            private Action _callback;

            public Callback(Action callback)
            {
                _callback = callback;
            }

            protected override void OnDisposed()
            {
                base.OnDisposed();
                _callback();
            }
        }
    }
}
