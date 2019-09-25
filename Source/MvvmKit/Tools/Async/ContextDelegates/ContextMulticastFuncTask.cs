using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ContextMulticastFuncTask
    {
        private HashSet<ContextFunc<Task>> _actions;

        public ContextMulticastFuncTask()
        {
            _actions = new HashSet<ContextFunc<Task>>();
        }

        private ContextMulticastFuncTask(IEnumerable<ContextFunc<Task>> actions)
        {
            _actions = new HashSet<ContextFunc<Task>>(actions);
        }

        public ContextFunc<Task>[] GetInvocationList()
        {
            return _actions.ToArray();
        }

        public Task Invoke()
        {
            _actions.RemoveWhere(ca => !ca.IsAlive);
            var tasks = _actions.Select(a => a.Invoke().Unwrap());
            return Task.WhenAll(tasks);
        }

        public ContextMulticastFuncTask Add(ContextFunc<Task> ca)
        {
            return new ContextMulticastFuncTask(_actions.Concat(ca));
        }

        public static ContextMulticastFuncTask operator+(ContextMulticastFuncTask cma, ContextFunc<Task> ca)
        {
            return cma.Add(ca);
        }

        public ContextMulticastFuncTask Add(AsyncContextRunner runner, WeakFunc<Task> wa)
        {
            return new ContextMulticastFuncTask(_actions.Concat(wa.InContext(runner)));
        }

        public static ContextMulticastFuncTask operator+(ContextMulticastFuncTask cma, (AsyncContextRunner runner, WeakFunc<Task> wa) action)
        {
            return cma.Add(action.runner, action.wa);
        }

        public ContextMulticastFuncTask Add(AsyncContextRunner runner, object owner, Func<Task> a)
        {
            return new ContextMulticastFuncTask(_actions.Concat(a.InContext(runner, owner)));
        }

        public static ContextMulticastFuncTask operator +(ContextMulticastFuncTask cma, (AsyncContextRunner runner, object owner, Func<Task> a) action)
        {
            return cma.Add(action.runner, action.owner, action.a);
        }

        public ContextMulticastFuncTask Add(TaskScheduler scheduler, WeakFunc<Task> wa)
        {
            return new ContextMulticastFuncTask(_actions.Concat(wa.InContext(scheduler)));
        }

        public static ContextMulticastFuncTask operator +(ContextMulticastFuncTask cma, (TaskScheduler scheduler, WeakFunc<Task> wa) action)
        {
            return cma.Add(action.scheduler, action.wa);
        }

        public ContextMulticastFuncTask Add(TaskScheduler scheduler, object owner, Func<Task> a)
        {
            return new ContextMulticastFuncTask(_actions.Concat(a.InContext(scheduler, owner)));
        }

        public static ContextMulticastFuncTask operator +(ContextMulticastFuncTask cma, (TaskScheduler scheduler, object owner, Func<Task> a) action)
        {
            return cma.Add(action.scheduler, action.owner, action.a);
        }

        public ContextMulticastFuncTask Add(WeakFunc<Task> wa)
        {
            return new ContextMulticastFuncTask(_actions.Concat(wa.InContext()));
        }

        public static ContextMulticastFuncTask operator +(ContextMulticastFuncTask cma, WeakFunc<Task> wa)
        {
            return cma.Add(wa);
        }

        public ContextMulticastFuncTask Add(object owner, Func<Task> a)
        {
            return new ContextMulticastFuncTask(_actions.Concat(a.InContext(owner)));
        }

        public static ContextMulticastFuncTask operator +(ContextMulticastFuncTask cma, (object owner, Func<Task> a) action)
        {
            return cma.Add(action.owner, action.a);
        }


        public ContextMulticastFuncTask Remove(ContextFunc<Task> ca)
        {
            return new ContextMulticastFuncTask(_actions.Where(a => a != ca));
        }

        public static ContextMulticastFuncTask operator -(ContextMulticastFuncTask cma, ContextFunc<Task> action)
        {
            return cma.Remove(action);
        }

        public ContextMulticastFuncTask Remove(TaskScheduler scheduler, WeakFunc<Task> wa)
        {
            return new ContextMulticastFuncTask(_actions.Where(a => (a.WeakFunc !=wa) || (a.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastFuncTask operator -(ContextMulticastFuncTask cma, (TaskScheduler scheduler, WeakFunc<Task> wa) action)
        {
            return cma.Remove(action.scheduler, action.wa);
        }

        public ContextMulticastFuncTask Remove(AsyncContextRunner runner, WeakFunc<Task> wa)
        {
            return new ContextMulticastFuncTask(_actions.Where(a => (a.WeakFunc !=wa) || (a.ContextRunner != runner)));
        }

        public static ContextMulticastFuncTask operator -(ContextMulticastFuncTask cma, (AsyncContextRunner runner, WeakFunc<Task> wa) action)
        {
            return cma.Remove(action.runner, action.wa);
        }

        public ContextMulticastFuncTask Remove(WeakFunc<Task> wa)
        {
            return new ContextMulticastFuncTask(_actions.Where(a => a.WeakFunc !=wa));
        }

        public static ContextMulticastFuncTask operator -(ContextMulticastFuncTask cma, WeakFunc<Task> wa)
        {
            return cma.Remove(wa);
        }

        public ContextMulticastFuncTask Remove(TaskScheduler scheduler, object owner, Func<Task> a)
        {
            return new ContextMulticastFuncTask(_actions.Where(ac =>
            (ac.Method != a.Method)
            || (ac.WeakFunc.Owner != owner)
            || (ac.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastFuncTask operator -(ContextMulticastFuncTask cma, (TaskScheduler scheduler, object owner, Func<Task> a) action)
        {
            return cma.Remove(action.scheduler, action.owner, action.a);
        }

        public ContextMulticastFuncTask Remove(TaskScheduler scheduler, object owner)
        {
            return new ContextMulticastFuncTask(_actions.Where(ac =>
            (ac.WeakFunc.Owner != owner)
            || (ac.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastFuncTask operator -(ContextMulticastFuncTask cma, (TaskScheduler scheduler, object owner) action)
        {
            return cma.Remove(action.scheduler, action.owner);
        }

        public ContextMulticastFuncTask Remove(AsyncContextRunner runner, object owner, Func<Task> a)
        {
            return new ContextMulticastFuncTask(_actions.Where(ac =>
            (ac.Method != a.Method)
            || (ac.WeakFunc.Owner != owner)
            || (ac.ContextRunner != runner)));
        }

        public static ContextMulticastFuncTask operator -(ContextMulticastFuncTask cma, (AsyncContextRunner runner, object owner, Func<Task> a) action)
        {
            return cma.Remove(action.runner, action.owner, action.a);
        }

        public ContextMulticastFuncTask Remove(AsyncContextRunner runner, object owner)
        {
            return new ContextMulticastFuncTask(_actions.Where(ac =>
            (ac.WeakFunc.Owner != owner)
            || (ac.ContextRunner != runner)));
        }

        public static ContextMulticastFuncTask operator -(ContextMulticastFuncTask cma, (AsyncContextRunner runner, object owner) action)
        {
            return cma.Remove(action.runner, action.owner);
        }

        public ContextMulticastFuncTask Remove(object owner, Func<Task> a)
        {
            return new ContextMulticastFuncTask(_actions.Where(ac =>
                (ac.Method != a.Method) ||
                (ac.Owner != owner)));
        }

        public static ContextMulticastFuncTask operator -(ContextMulticastFuncTask cma, (object owner, Func<Task> callback) action)
        {
            return cma.Remove(action.owner, action.callback);
        }

        public ContextMulticastFuncTask Remove(object owner)
        {
            return new ContextMulticastFuncTask(_actions.Where(ac => ac.WeakFunc.Owner != owner));
        }

        public static ContextMulticastFuncTask operator -(ContextMulticastFuncTask cma, object owner)
        {
            return cma.Remove(owner);
        }


    }
}
