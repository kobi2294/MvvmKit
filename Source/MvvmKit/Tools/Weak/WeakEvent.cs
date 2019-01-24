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

        public static WeakEvent operator+ (WeakEvent e, (object owner, Action<object> action) listener)
        {
            var listeners = new HashSet<WeakAction<object>>(e._listeners);
            listeners.Add(listener.action.ToWeak(listener.owner));
            return new WeakEvent(listeners);
        }

        public static WeakEvent operator-(WeakEvent e, (object owner, Action<object> action) listener)
        {
            var listeners = new HashSet<WeakAction<object>>(e._listeners);
            listeners.RemoveWhere(wa => (wa.Owner == listener.owner) && (wa.Method == listener.action.Method));
            return new WeakEvent(listeners);
        }

        public static WeakEvent operator-(WeakEvent e, object owner)
        {
            var listeners = new HashSet<WeakAction<object>>(e._listeners);
            listeners.RemoveWhere(wa => (wa.Owner == owner));
            return new WeakEvent(listeners);
        }

        public  WeakEvent Add(object owner, Action<object> action)
        {
            return this + (owner, action);
        }

        public  WeakEvent Remove(object owner, Action<object> action)
        {
            return this - (owner, action);
        }

        public  WeakEvent Remove(object owner)
        {
            return this - owner;
        }
    }
}
