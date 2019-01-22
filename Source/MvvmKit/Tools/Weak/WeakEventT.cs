using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class WeakEvent<T>
    {
        private HashSet<WeakAction<object, T>> _listeners;

        public void Invoke(object sender, T arg)
        {
            // remove all dead references
            _listeners.RemoveWhere(l => !l.IsAlive);

            foreach (var listener in _listeners)
            {
                listener.Execute(sender, arg);
            }
        }

        public WeakEvent()
        {
            _listeners = new HashSet<WeakAction<object, T>>();
        }

        private WeakEvent(HashSet<WeakAction<object, T>> listeners)
        {
            _listeners = listeners;
        }

        public static WeakEvent<T> operator +(WeakEvent<T> e, (object observer, Action<object, T> action) listener)
        {
            var listeners = new HashSet<WeakAction<object, T>>(e._listeners);
            listeners.Add(new WeakAction<object, T>(listener.observer, listener.action));
            return new WeakEvent<T>(listeners);
        }

        public static WeakEvent<T> operator -(WeakEvent<T> e, (object observer, Action<object, T> action) listener)
        {
            var listeners = new HashSet<WeakAction<object, T>>(e._listeners);
            listeners.RemoveWhere(wa => (wa.Target == listener.observer) && (wa.Method == listener.action.Method));
            return new WeakEvent<T>(listeners);
        }

        public static WeakEvent<T> operator -(WeakEvent<T> e, object observer)
        {
            var listeners = new HashSet<WeakAction<object, T>>(e._listeners);
            listeners.RemoveWhere(wa => (wa.Target == observer));
            return new WeakEvent<T>(listeners);
        }
    }
}
