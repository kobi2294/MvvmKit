using System;
using System.Collections;
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
        private HashSet<WeakReference<IEnumerable>> _weakCollections;
        private HashSet<IEnumerable> _strongCollections;

        public NotifyDisposableHandler()
        {
            _weakChildren = new HashSet<WeakReference<IDisposable>>();
            _strongChildren = new HashSet<IDisposable>();
            _weakCollections = new HashSet<WeakReference<IEnumerable>>();
            _strongCollections = new HashSet<IEnumerable>();
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

        public void AttachMany<T>(IEnumerable<T> children, bool keepAlive = false) 
            where T : IDisposable
        {
            if (!keepAlive)
            {
                _weakCollections.Add(new WeakReference<IEnumerable>(children));
            } else
            {
                _strongCollections.Add(children);
            }
        }

        public void Dettach(IDisposable child)
        {
            _weakChildren.RemoveWhere(wr => !wr.TryGetTarget(out var target) || ReferenceEquals(target, child));
            _strongChildren.Remove(child);
        }

        public void DettachMany<T>(IEnumerable<T> children) 
            where T : IDisposable
        {
            _weakCollections.RemoveWhere(wr => !wr.TryGetTarget(out var target) || ReferenceEquals(target, children));
            _strongCollections.Remove(children);
        }

        public void Dispose()
        {
            IsDisposed = true;
            foreach (var childRef in _weakChildren.ToList())
            {
                if (childRef.TryGetTarget(out var child))
                {
                    child?.Dispose();
                }
            }

            foreach (var child in _strongChildren.ToList())
            {
                child?.Dispose();
            }

            foreach (var collectionRef in _weakCollections.ToList())
            {
                if (collectionRef.TryGetTarget(out var collection))
                {
                    if (collection != null)
                    {
                        foreach (IDisposable child in collection)
                        {
                            child?.Dispose();
                        }
                    }
                }
            }

            foreach (var collection in _strongCollections.ToList()) 
            {
                if (collection != null)
                {
                    foreach (IDisposable child in collection)
                    {
                        child?.Dispose();
                    }
                }
            }

            _weakChildren.Clear();
            _strongChildren.Clear();
            _weakCollections.Clear();
            _strongCollections.Clear();
        }

    }
}
