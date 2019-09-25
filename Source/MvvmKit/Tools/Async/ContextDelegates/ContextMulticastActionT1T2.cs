using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ContextMulticastAction<T1, T2>
    {
        private HashSet<ContextAction<T1, T2>> _actions;

        public ContextMulticastAction()
        {
            _actions = new HashSet<ContextAction<T1, T2>>();
        }

        private ContextMulticastAction(IEnumerable<ContextAction<T1, T2>> actions)
        {
            _actions = new HashSet<ContextAction<T1, T2>>(actions);
        }

        public ContextAction<T1, T2>[] GetInvocationList()
        {
            return _actions.ToArray();
        }

        public Task Invoke(T1 arg1, T2 arg2)
        {
            _actions.RemoveWhere(ca => !ca.IsAlive);
            var tasks = _actions.Select(a => a.Invoke(arg1, arg2));
            return Task.WhenAll(tasks);
        }

        public ContextMulticastAction<T1, T2> Add(ContextAction<T1, T2> ca)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Concat(ca));
        }

        public static ContextMulticastAction<T1, T2> operator+(ContextMulticastAction<T1, T2> cma, ContextAction<T1, T2> ca)
        {
            return cma.Add(ca);
        }

        public ContextMulticastAction<T1, T2> Add(WeakAction<T1, T2> wa, AsyncContextRunner runner)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Concat(wa.InContext(runner)));
        }

        public static ContextMulticastAction<T1, T2> operator+(ContextMulticastAction<T1, T2> cma, (WeakAction<T1, T2> wa, AsyncContextRunner runner) action)
        {
            return cma.Add(action.wa, action.runner);
        }

        public ContextMulticastAction<T1, T2> Add(Action<T1, T2> a, object owner, AsyncContextRunner runner)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Concat(a.InContext(owner, runner)));
        }

        public static ContextMulticastAction<T1, T2> operator +(ContextMulticastAction<T1, T2> cma, (Action<T1, T2> a, object owner, AsyncContextRunner runner) action)
        {
            return cma.Add(action.a, action.owner, action.runner);
        }

        public ContextMulticastAction<T1, T2> Add(WeakAction<T1, T2> wa, TaskScheduler scheduler)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Concat(wa.InContext(scheduler)));
        }

        public static ContextMulticastAction<T1, T2> operator +(ContextMulticastAction<T1, T2> cma, (WeakAction<T1, T2> wa, TaskScheduler scheduler) action)
        {
            return cma.Add(action.wa, action.scheduler);
        }

        public ContextMulticastAction<T1, T2> Add(Action<T1, T2> a, object owner, TaskScheduler scheduler)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Concat(a.InContext(owner, scheduler)));
        }

        public static ContextMulticastAction<T1, T2> operator +(ContextMulticastAction<T1, T2> cma, (Action<T1, T2> a, object owner, TaskScheduler scheduler) action)
        {
            return cma.Add(action.a, action.owner, action.scheduler);
        }

        public ContextMulticastAction<T1, T2> Add(WeakAction<T1, T2> wa)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Concat(wa.InContext()));
        }

        public static ContextMulticastAction<T1, T2> operator +(ContextMulticastAction<T1, T2> cma, WeakAction<T1, T2> wa)
        {
            return cma.Add(wa);
        }

        public ContextMulticastAction<T1, T2> Add(Action<T1, T2> a, object owner)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Concat(a.InContext(owner)));
        }

        public static ContextMulticastAction<T1, T2> operator +(ContextMulticastAction<T1, T2> cma, (Action<T1, T2> a, object owner) action)
        {
            return cma.Add(action.a, action.owner);
        }


        public ContextMulticastAction<T1, T2> Remove(ContextAction<T1, T2> ca)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Where(a => a != ca));
        }

        public static ContextMulticastAction<T1, T2> operator -(ContextMulticastAction<T1, T2> cma, ContextAction<T1, T2> action)
        {
            return cma.Remove(action);
        }

        public ContextMulticastAction<T1, T2> Remove(WeakAction<T1, T2> wa, TaskScheduler scheduler)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Where(a => (a.WeakAction != wa) || (a.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastAction<T1, T2> operator -(ContextMulticastAction<T1, T2> cma, (WeakAction<T1, T2> wa, TaskScheduler scheduler) action)
        {
            return cma.Remove(action.wa, action.scheduler);
        }

        public ContextMulticastAction<T1, T2> Remove(WeakAction<T1, T2> wa, AsyncContextRunner runner)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Where(a => (a.WeakAction != wa) || (a.ContextRunner != runner)));
        }

        public static ContextMulticastAction<T1, T2> operator -(ContextMulticastAction<T1, T2> cma, (WeakAction<T1, T2> wa, AsyncContextRunner runner) action)
        {
            return cma.Remove(action.wa, action.runner);
        }

        public ContextMulticastAction<T1, T2> Remove(WeakAction<T1, T2> wa)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Where(a => a.WeakAction != wa));
        }

        public static ContextMulticastAction<T1, T2> operator -(ContextMulticastAction<T1, T2> cma, WeakAction<T1, T2> wa)
        {
            return cma.Remove(wa);
        }

        public ContextMulticastAction<T1, T2> Remove(Action<T1, T2> a, object owner, TaskScheduler scheduler)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Where(ac =>
            (ac.Method != a.Method)
            || (ac.WeakAction.Owner != owner)
            || (ac.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastAction<T1, T2> operator -(ContextMulticastAction<T1, T2> cma, (Action<T1, T2> a, object owner, TaskScheduler scheduler) action)
        {
            return cma.Remove(action.a, action.owner, action.scheduler);
        }

        public ContextMulticastAction<T1, T2> Remove(object owner, TaskScheduler scheduler)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Where(ac =>
            (ac.WeakAction.Owner != owner)
            || (ac.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastAction<T1, T2> operator -(ContextMulticastAction<T1, T2> cma, (object owner, TaskScheduler scheduler) action)
        {
            return cma.Remove(action.owner, action.scheduler);
        }

        public ContextMulticastAction<T1, T2> Remove(Action<T1, T2> a, object owner, AsyncContextRunner runner)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Where(ac =>
            (ac.Method != a.Method)
            || (ac.WeakAction.Owner != owner)
            || (ac.ContextRunner != runner)));
        }

        public static ContextMulticastAction<T1, T2> operator -(ContextMulticastAction<T1, T2> cma, (Action<T1, T2> a, object owner, AsyncContextRunner runner) action)
        {
            return cma.Remove(action.a, action.owner, action.runner);
        }

        public ContextMulticastAction<T1, T2> Remove(object owner, AsyncContextRunner runner)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Where(ac =>
            (ac.WeakAction.Owner != owner)
            || (ac.ContextRunner != runner)));
        }

        public static ContextMulticastAction<T1, T2> operator -(ContextMulticastAction<T1, T2> cma, (object owner, AsyncContextRunner runner) action)
        {
            return cma.Remove(action.owner, action.runner);
        }

        public ContextMulticastAction<T1, T2> Remove(object owner, Action<T1, T2> a)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Where(ac =>
                (ac.Method != a.Method) ||
                (ac.Owner != owner)));
        }

        public static ContextMulticastAction<T1, T2> operator -(ContextMulticastAction<T1, T2> cma, (object owner, Action<T1, T2> callback) action)
        {
            return cma.Remove(action.owner, action.callback);
        }


        public ContextMulticastAction<T1, T2> Remove(object owner)
        {
            return new ContextMulticastAction<T1, T2>(_actions.Where(ac => ac.WeakAction.Owner != owner));
        }

        public static ContextMulticastAction<T1, T2> operator -(ContextMulticastAction<T1, T2> cma, object owner)
        {
            return cma.Remove(owner);
        }


    }
}
