using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ContextMulticastAction<T>
    {
        private HashSet<ContextAction<T>> _actions;

        public ContextMulticastAction()
        {
            _actions = new HashSet<ContextAction<T>>();
        }

        private ContextMulticastAction(IEnumerable<ContextAction<T>> actions)
        {
            _actions = new HashSet<ContextAction<T>>(actions);
        }

        public ContextAction<T>[] GetInvocationList()
        {
            return _actions.ToArray();
        }

        public Task Invoke(T arg)
        {
            _actions.RemoveWhere(ca => !ca.IsAlive);
            var tasks = _actions.Select(a => a.Invoke(arg));
            return Task.WhenAll(tasks);
        }

        public ContextMulticastAction<T> Add(ContextAction<T> ca)
        {
            return new ContextMulticastAction<T>(_actions.Concat(ca));
        }

        public static ContextMulticastAction<T> operator+(ContextMulticastAction<T> cma, ContextAction<T> ca)
        {
            return cma.Add(ca);
        }

        public ContextMulticastAction<T> Add(WeakAction<T> wa, AsyncContextRunner runner)
        {
            return new ContextMulticastAction<T>(_actions.Concat(wa.InContext(runner)));
        }

        public static ContextMulticastAction<T> operator+(ContextMulticastAction<T> cma, (WeakAction<T> wa, AsyncContextRunner runner) action)
        {
            return cma.Add(action.wa, action.runner);
        }

        public ContextMulticastAction<T> Add(Action<T> a, object owner, AsyncContextRunner runner)
        {
            return new ContextMulticastAction<T>(_actions.Concat(a.InContext(owner, runner)));
        }

        public static ContextMulticastAction<T> operator +(ContextMulticastAction<T> cma, (Action<T> a, object owner, AsyncContextRunner runner) action)
        {
            return cma.Add(action.a, action.owner, action.runner);
        }

        public ContextMulticastAction<T> Add(WeakAction<T> wa, TaskScheduler scheduler)
        {
            return new ContextMulticastAction<T>(_actions.Concat(wa.InContext(scheduler)));
        }

        public static ContextMulticastAction<T> operator +(ContextMulticastAction<T> cma, (WeakAction<T> wa, TaskScheduler scheduler) action)
        {
            return cma.Add(action.wa, action.scheduler);
        }

        public ContextMulticastAction<T> Add(Action<T> a, object owner, TaskScheduler scheduler)
        {
            return new ContextMulticastAction<T>(_actions.Concat(a.InContext(owner, scheduler)));
        }

        public static ContextMulticastAction<T> operator +(ContextMulticastAction<T> cma, (Action<T> a, object owner, TaskScheduler scheduler) action)
        {
            return cma.Add(action.a, action.owner, action.scheduler);
        }

        public ContextMulticastAction<T> Add(WeakAction<T> wa)
        {
            return new ContextMulticastAction<T>(_actions.Concat(wa.InContext()));
        }

        public static ContextMulticastAction<T> operator +(ContextMulticastAction<T> cma, WeakAction<T> wa)
        {
            return cma.Add(wa);
        }

        public ContextMulticastAction<T> Add(Action<T> a, object owner)
        {
            return new ContextMulticastAction<T>(_actions.Concat(a.InContext(owner)));
        }

        public static ContextMulticastAction<T> operator +(ContextMulticastAction<T> cma, (Action<T> a, object owner) action)
        {
            return cma.Add(action.a, action.owner);
        }


        public ContextMulticastAction<T> Remove(ContextAction<T> ca)
        {
            return new ContextMulticastAction<T>(_actions.Where(a => a != ca));
        }

        public static ContextMulticastAction<T> operator -(ContextMulticastAction<T> cma, ContextAction<T> action)
        {
            return cma.Remove(action);
        }

        public ContextMulticastAction<T> Remove(WeakAction<T> wa, TaskScheduler scheduler)
        {
            return new ContextMulticastAction<T>(_actions.Where(a => (a.WeakAction != wa) || (a.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastAction<T> operator -(ContextMulticastAction<T> cma, (WeakAction<T> wa, TaskScheduler scheduler) action)
        {
            return cma.Remove(action.wa, action.scheduler);
        }

        public ContextMulticastAction<T> Remove(WeakAction<T> wa, AsyncContextRunner runner)
        {
            return new ContextMulticastAction<T>(_actions.Where(a => (a.WeakAction != wa) || (a.ContextRunner != runner)));
        }

        public static ContextMulticastAction<T> operator -(ContextMulticastAction<T> cma, (WeakAction<T> wa, AsyncContextRunner runner) action)
        {
            return cma.Remove(action.wa, action.runner);
        }

        public ContextMulticastAction<T> Remove(WeakAction<T> wa)
        {
            return new ContextMulticastAction<T>(_actions.Where(a => a.WeakAction != wa));
        }

        public static ContextMulticastAction<T> operator -(ContextMulticastAction<T> cma, WeakAction<T> wa)
        {
            return cma.Remove(wa);
        }

        public ContextMulticastAction<T> Remove(Action<T> a, object owner, TaskScheduler scheduler)
        {
            return new ContextMulticastAction<T>(_actions.Where(ac =>
            (ac.Method != a.Method)
            || (ac.WeakAction.Owner != owner)
            || (ac.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastAction<T> operator -(ContextMulticastAction<T> cma, (Action<T> a, object owner, TaskScheduler scheduler) action)
        {
            return cma.Remove(action.a, action.owner, action.scheduler);
        }

        public ContextMulticastAction<T> Remove(object owner, TaskScheduler scheduler)
        {
            return new ContextMulticastAction<T>(_actions.Where(ac =>
            (ac.WeakAction.Owner != owner)
            || (ac.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastAction<T> operator -(ContextMulticastAction<T> cma, (object owner, TaskScheduler scheduler) action)
        {
            return cma.Remove(action.owner, action.scheduler);
        }

        public ContextMulticastAction<T> Remove(Action<T> a, object owner, AsyncContextRunner runner)
        {
            return new ContextMulticastAction<T>(_actions.Where(ac =>
            (ac.Method != a.Method)
            || (ac.WeakAction.Owner != owner)
            || (ac.ContextRunner != runner)));
        }

        public static ContextMulticastAction<T> operator -(ContextMulticastAction<T> cma, (Action<T> a, object owner, AsyncContextRunner runner) action)
        {
            return cma.Remove(action.a, action.owner, action.runner);
        }

        public ContextMulticastAction<T> Remove(object owner, AsyncContextRunner runner)
        {
            return new ContextMulticastAction<T>(_actions.Where(ac =>
            (ac.WeakAction.Owner != owner)
            || (ac.ContextRunner != runner)));
        }

        public static ContextMulticastAction<T> operator -(ContextMulticastAction<T> cma, (object owner, AsyncContextRunner runner) action)
        {
            return cma.Remove(action.owner, action.runner);
        }

        public ContextMulticastAction<T> Remove(object owner)
        {
            return new ContextMulticastAction<T>(_actions.Where(ac => ac.WeakAction.Owner != owner));
        }

        public static ContextMulticastAction<T> operator -(ContextMulticastAction<T> cma, object owner)
        {
            return cma.Remove(owner);
        }


    }
}
