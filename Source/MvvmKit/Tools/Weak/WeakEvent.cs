using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class WeakEvent
    {
        private HashSet<WeakAction<object>> _listeners;

        public void Invoke(object sender)
        {
            // remove all dead references
            _listeners.RemoveWhere(l => !l.IsAlive);

            foreach (var listener in _listeners)
            {
                listener.Execute(sender);
            }
        }

        public WeakEvent()
        {
            _listeners = new HashSet<WeakAction<object>>();
        }

        private WeakEvent(HashSet<WeakAction<object>> listeners)
        {
            _listeners = listeners;
        }

        public static WeakEvent operator+ (WeakEvent e, (object observer, Action<object> action) listener)
        {
            var listeners = new HashSet<WeakAction<object>>(e._listeners);
            listeners.Add(new WeakAction<object>(listener.observer, listener.action));
            return new WeakEvent(listeners);
        }

        public static WeakEvent operator-(WeakEvent e, (object observer, Action<object> action) listener)
        {
            var listeners = new HashSet<WeakAction<object>>(e._listeners);
            listeners.RemoveWhere(wa => (wa.Target == listener.observer) && (wa.Method == listener.action.Method));
            return new WeakEvent(listeners);
        }

        public static WeakEvent operator-(WeakEvent e, object observer)
        {
            var listeners = new HashSet<WeakAction<object>>(e._listeners);
            listeners.RemoveWhere(wa => (wa.Target == observer));
            return new WeakEvent(listeners);
        }
    }
}
