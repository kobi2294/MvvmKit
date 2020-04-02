using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class PropertyChangeListener: BaseDisposable
    {
        private HashSet<WeakAction> _listeners;

        private HashSet<WeakAction> _ensureListeners()
        {
            if (_listeners == null)
                _listeners = new HashSet<WeakAction>();

            return _listeners;
        }

        private void _invoke(WeakAction listener, object oldValue, object newValue)
        {
            if (listener is IWeakActionWithParam2)
                (listener as IWeakActionWithParam2).ExecuteWithObject(oldValue, newValue);
            else if (listener is IWeakActionWithParam1)
                (listener as IWeakActionWithParam1).ExecuteWithObject(newValue);
            else
                listener.Execute();
        }

        private void _invoke(object oldValue, object newValue)
        {
            var listeners = _ensureListeners();
            listeners.RemoveWhere(wa => !wa.IsAlive);
           
            foreach (var listener in listeners.ToList())
            {
                _invoke(listener, oldValue, newValue);
            }
        }

        internal void Invoke(object oldValue, object newValue)
        {
            _invoke(oldValue, newValue);
        }

        internal void Observe(object owner, Action a)
        {
            _ensureListeners().Add(new WeakAction(owner, a));
        }

        internal void Observe<T>(object owner, Action<T> a)
        {
            _ensureListeners().Add(new WeakAction<T>(owner, a));
        }

        internal void Observe<T>(object owner, Action<T, T> a)
        {
            _ensureListeners().Add(new WeakAction<T, T>(owner, a));
        }

        internal void Unobserve(object owner)
        {
            _ensureListeners().RemoveWhere(wa => wa.Owner == owner);
        }

        internal void Unobserve(object owner, Action a)
        {
            _ensureListeners().RemoveWhere(wa => (wa.Owner == owner) && (wa.Method == a.Method));
        }

        internal void Unobserve<T>(object owner, Action<T> a)
        {
            _ensureListeners().RemoveWhere(wa => (wa.Owner == owner) && (wa.Method == a.Method));
        }

        internal void Unobserve<T>(object owner, Action<T, T> a)
        {
            _ensureListeners().RemoveWhere(wa => (wa.Owner == owner) && (wa.Method == a.Method));
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            _listeners = null;
        }

    }
}
