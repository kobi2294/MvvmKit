using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class BaseDisposable : INotifyDisposable
    {
        private INotifyDisposable _handler = MvvmKit.Disposables.Notifier();

        public bool IsDisposed => _handler.IsDisposed;

        public void Attach(IDisposable child, bool keepAlive = false) => _handler.Attach(child, keepAlive);

        public void Dettach(IDisposable child) => _handler.Dettach(child);

        public void AttachMany<T>(IEnumerable<T> children, bool keepAlive = false)
            where T : IDisposable
        {
            _handler.AttachMany(children, keepAlive);
        }

        public void DettachMany<T>(IEnumerable<T> children)
            where T : IDisposable
        {
            _handler.DettachMany(children);
        }

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
            if (IsDisposed) return;
            _handler.Dispose();
            OnDisposed();
        }
    }
}
