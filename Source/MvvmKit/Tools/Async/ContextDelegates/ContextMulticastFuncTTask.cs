using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ContextMulticastFuncTask<T>
    {
        private HashSet<ContextFunc<T, Task>> _actions;

        public ContextMulticastFuncTask()
        {
            _actions = new HashSet<ContextFunc<T, Task>>();
        }

        private ContextMulticastFuncTask(IEnumerable<ContextFunc<T, Task>> actions)
        {
            _actions = new HashSet<ContextFunc<T, Task>>(actions);
        }

        public ContextFunc<T, Task>[] GetInvocationList()
        {
            return _actions.ToArray();
        }

        public Task Invoke(T arg)
        {
            _actions.RemoveWhere(ca => !ca.IsAlive);
            var tasks = _actions.Select(a => a.Invoke(arg).Unwrap());
            return Task.WhenAll(tasks);
        }

        public ContextMulticastFuncTask<T> Add(ContextFunc<T, Task> ca)
        {
            return new ContextMulticastFuncTask<T>(_actions.Concat(ca));
        }

        public static ContextMulticastFuncTask<T> operator +(ContextMulticastFuncTask<T> cma, ContextFunc<T, Task> ca)
        {
            return cma.Add(ca);
        }

        public ContextMulticastFuncTask<T> Add(AsyncContextRunner runner, WeakFunc<T, Task> wa)
        {
            return new ContextMulticastFuncTask<T>(_actions.Concat(wa.InContext(runner)));
        }

        public static ContextMulticastFuncTask<T> operator +(ContextMulticastFuncTask<T> cma, (AsyncContextRunner runner, WeakFunc<T, Task> wa) action)
        {
            return cma.Add(action.runner, action.wa);
        }

        public ContextMulticastFuncTask<T> Add(AsyncContextRunner runner, object owner, Func<T, Task> a)
        {
            return new ContextMulticastFuncTask<T>(_actions.Concat(a.InContext(runner, owner)));
        }

        public static ContextMulticastFuncTask<T> operator +(ContextMulticastFuncTask<T> cma, (AsyncContextRunner runner, object owner, Func<T, Task> a) action)
        {
            return cma.Add(action.runner, action.owner, action.a);
        }

        public ContextMulticastFuncTask<T> Add(TaskScheduler scheduler, WeakFunc<T, Task> wa)
        {
            return new ContextMulticastFuncTask<T>(_actions.Concat(wa.InContext(scheduler)));
        }

        public static ContextMulticastFuncTask<T> operator +(ContextMulticastFuncTask<T> cma, (TaskScheduler scheduler, WeakFunc<T, Task> wa) action)
        {
            return cma.Add(action.scheduler, action.wa);
        }

        public ContextMulticastFuncTask<T> Add(TaskScheduler scheduler, object owner, Func<T, Task> a)
        {
            return new ContextMulticastFuncTask<T>(_actions.Concat(a.InContext(scheduler, owner)));
        }

        public static ContextMulticastFuncTask<T> operator +(ContextMulticastFuncTask<T> cma, (TaskScheduler scheduler, object owner, Func<T, Task> a) action)
        {
            return cma.Add(action.scheduler, action.owner, action.a);
        }

        public ContextMulticastFuncTask<T> Add(WeakFunc<T, Task> wa)
        {
            return new ContextMulticastFuncTask<T>(_actions.Concat(wa.InContext()));
        }

        public static ContextMulticastFuncTask<T> operator +(ContextMulticastFuncTask<T> cma, WeakFunc<T, Task> wa)
        {
            return cma.Add(wa);
        }

        public ContextMulticastFuncTask<T> Add(object owner, Func<T, Task> a)
        {
            return new ContextMulticastFuncTask<T>(_actions.Concat(a.InContext(owner)));
        }

        public static ContextMulticastFuncTask<T> operator +(ContextMulticastFuncTask<T> cma, (object owner, Func<T, Task> a) action)
        {
            return cma.Add(action.owner, action.a);
        }


        public ContextMulticastFuncTask<T> Remove(ContextFunc<T, Task> ca)
        {
            return new ContextMulticastFuncTask<T>(_actions.Where(a => a != ca));
        }

        public static ContextMulticastFuncTask<T> operator -(ContextMulticastFuncTask<T> cma, ContextFunc<T, Task> action)
        {
            return cma.Remove(action);
        }

        public ContextMulticastFuncTask<T> Remove(TaskScheduler scheduler, WeakFunc<T, Task> wa)
        {
            return new ContextMulticastFuncTask<T>(_actions.Where(a => (a.WeakFunc !=wa) || (a.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastFuncTask<T> operator -(ContextMulticastFuncTask<T> cma, (TaskScheduler scheduler, WeakFunc<T, Task> wa) action)
        {
            return cma.Remove(action.scheduler, action.wa);
        }

        public ContextMulticastFuncTask<T> Remove(AsyncContextRunner runner, WeakFunc<T, Task> wa)
        {
            return new ContextMulticastFuncTask<T>(_actions.Where(a => (a.WeakFunc !=wa) || (a.ContextRunner != runner)));
        }

        public static ContextMulticastFuncTask<T> operator -(ContextMulticastFuncTask<T> cma, (AsyncContextRunner runner, WeakFunc<T, Task> wa) action)
        {
            return cma.Remove(action.runner, action.wa);
        }

        public ContextMulticastFuncTask<T> Remove(WeakFunc<T, Task> wa)
        {
            return new ContextMulticastFuncTask<T>(_actions.Where(a => a.WeakFunc !=wa));
        }

        public static ContextMulticastFuncTask<T> operator -(ContextMulticastFuncTask<T> cma, WeakFunc<T, Task> wa)
        {
            return cma.Remove(wa);
        }

        public ContextMulticastFuncTask<T> Remove(TaskScheduler scheduler, object owner, Func<T, Task> a)
        {
            return new ContextMulticastFuncTask<T>(_actions.Where(ac =>
            (ac.Method != a.Method)
            || (ac.WeakFunc.Owner != owner)
            || (ac.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastFuncTask<T> operator -(ContextMulticastFuncTask<T> cma, (TaskScheduler scheduler, object owner, Func<T, Task> a) action)
        {
            return cma.Remove(action.scheduler, action.owner, action.a);
        }

        public ContextMulticastFuncTask<T> Remove(TaskScheduler scheduler, object owner)
        {
            return new ContextMulticastFuncTask<T>(_actions.Where(ac =>
            (ac.WeakFunc.Owner != owner)
            || (ac.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastFuncTask<T> operator -(ContextMulticastFuncTask<T> cma, (TaskScheduler scheduler, object owner) action)
        {
            return cma.Remove(action.scheduler, action.owner);
        }

        public ContextMulticastFuncTask<T> Remove(AsyncContextRunner runner, object owner, Func<T, Task> a)
        {
            return new ContextMulticastFuncTask<T>(_actions.Where(ac =>
            (ac.Method != a.Method)
            || (ac.WeakFunc.Owner != owner)
            || (ac.ContextRunner != runner)));
        }

        public static ContextMulticastFuncTask<T> operator -(ContextMulticastFuncTask<T> cma, (AsyncContextRunner runner, object owner, Func<T, Task> a) action)
        {
            return cma.Remove(action.runner, action.owner, action.a);
        }

        public ContextMulticastFuncTask<T> Remove(AsyncContextRunner runner, object owner)
        {
            return new ContextMulticastFuncTask<T>(_actions.Where(ac =>
            (ac.WeakFunc.Owner != owner)
            || (ac.ContextRunner != runner)));
        }

        public static ContextMulticastFuncTask<T> operator -(ContextMulticastFuncTask<T> cma, (AsyncContextRunner runner, object owner) action)
        {
            return cma.Remove(action.runner, action.owner);
        }

        public ContextMulticastFuncTask<T> Remove(object owner)
        {
            return new ContextMulticastFuncTask<T>(_actions.Where(ac => ac.WeakFunc.Owner != owner));
        }

        public static ContextMulticastFuncTask<T> operator -(ContextMulticastFuncTask<T> cma, object owner)
        {
            return cma.Remove(owner);
        }


    }
}
