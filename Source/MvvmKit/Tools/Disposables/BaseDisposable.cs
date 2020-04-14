using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class BaseDisposable : INotifyDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        public event EventHandler Disposing;

        public void Validate()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected virtual void OnDisposed()
        {
        }

        public void DisposeIfNeeded()
        {
            if (!IsDisposed)
                Dispose();
        }

        public void Dispose()
        {
            Validate();
            IsDisposed = true;
            OnDisposed();
            Disposing?.Invoke(this, EventArgs.Empty);
            Disposing = null;
        }
    }
}
