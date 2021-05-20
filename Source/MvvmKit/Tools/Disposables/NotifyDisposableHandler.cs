using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    internal class NotifyDisposableHandler: INotifyDisposable
    {
        private HashSet<WeakReference<IDisposable>> _weakChildren;
        private HashSet<IDisposable> _strongChildren;

        public NotifyDisposableHandler()
        {
            _weakChildren = new HashSet<WeakReference<IDisposable>>();
            _strongChildren = new HashSet<IDisposable>();
        }

        public bool IsDisposed { get; private set; }

        public void Attach(IDisposable child, bool keepAlive = false)
        {
            if (!keepAlive)
            {
                _weakChildren.Add(new WeakReference<IDisposable>(child));
            }
            else
            {
                _strongChildren.Add(child);
            }
        }

        public void Dettach(IDisposable child)
        {
            _weakChildren.RemoveWhere(wr => !wr.TryGetTarget(out var target) || ReferenceEquals(target, child));
            _strongChildren.Remove(child);
        }

        public void Dispose()
        {
            IsDisposed = true;
            foreach (var child in _weakChildren.ToList())
            {
                if (child.TryGetTarget(out var target))
                {
                    target?.Dispose();
                }
            }

            foreach (var child in _strongChildren.ToList())
            {
                child?.Dispose();
            }

            _weakChildren.Clear();
            _strongChildren.Clear();
        }

    }
}
