using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ContextMulticastAction
    {
        private HashSet<ContextAction> _actions;

        public ContextMulticastAction()
        {
            _actions = new HashSet<ContextAction>();
        }

        private ContextMulticastAction(IEnumerable<ContextAction> actions)
        {
            _actions = new HashSet<ContextAction>(actions);
        }

        public ContextAction[] GetInvocationList()
        {
            return _actions.ToArray();
        }

        public Task Invoke()
        {
            _actions.RemoveWhere(ca => !ca.IsAlive);
            var tasks = _actions.Select(a => a.Invoke());
            return Task.WhenAll(tasks);
        }

        public ContextMulticastAction Add(ContextAction ca)
        {
            return new ContextMulticastAction(_actions.Concat(ca));
        }

        public static ContextMulticastAction operator+(ContextMulticastAction cma, ContextAction ca)
        {
            return cma.Add(ca);
        }

        public ContextMulticastAction Add(WeakAction wa, AsyncContextRunner runner)
        {
            return new ContextMulticastAction(_actions.Concat(wa.InContext(runner)));
        }

        public static ContextMulticastAction operator+(ContextMulticastAction cma, (WeakAction wa, AsyncContextRunner runner) action)
        {
            return cma.Add(action.wa, action.runner);
        }

        public ContextMulticastAction Add(Action a, object owner, AsyncContextRunner runner)
        {
            return new ContextMulticastAction(_actions.Concat(a.InContext(owner, runner)));
        }

        public static ContextMulticastAction operator +(ContextMulticastAction cma, (Action a, object owner, AsyncContextRunner runner) action)
        {
            return cma.Add(action.a, action.owner, action.runner);
        }

        public ContextMulticastAction Add(WeakAction wa, TaskScheduler scheduler)
        {
            return new ContextMulticastAction(_actions.Concat(wa.InContext(scheduler)));
        }

        public static ContextMulticastAction operator +(ContextMulticastAction cma, (WeakAction wa, TaskScheduler scheduler) action)
        {
            return cma.Add(action.wa, action.scheduler);
        }

        public ContextMulticastAction Add(Action a, object owner, TaskScheduler scheduler)
        {
            return new ContextMulticastAction(_actions.Concat(a.InContext(owner, scheduler)));
        }

        public static ContextMulticastAction operator +(ContextMulticastAction cma, (Action a, object owner, TaskScheduler scheduler) action)
        {
            return cma.Add(action.a, action.owner, action.scheduler);
        }

        public ContextMulticastAction Add(WeakAction wa)
        {
            return new ContextMulticastAction(_actions.Concat(wa.InContext()));
        }

        public static ContextMulticastAction operator +(ContextMulticastAction cma, WeakAction wa)
        {
            return cma.Add(wa);
        }

        public ContextMulticastAction Add(Action a, object owner)
        {
            return new ContextMulticastAction(_actions.Concat(a.InContext(owner)));
        }

        public static ContextMulticastAction operator +(ContextMulticastAction cma, (Action a, object owner) action)
        {
            return cma.Add(action.a, action.owner);
        }

        public ContextMulticastAction Remove(ContextAction ca)
        {
            return new ContextMulticastAction(_actions.Where(a => a != ca));
        }

        public static ContextMulticastAction operator -(ContextMulticastAction cma, ContextAction action)
        {
            return cma.Remove(action);
        }

        public ContextMulticastAction Remove(WeakAction wa, TaskScheduler scheduler)
        {
            return new ContextMulticastAction(_actions.Where(a => (a.WeakAction != wa) || (a.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastAction operator -(ContextMulticastAction cma, (WeakAction wa, TaskScheduler scheduler) action)
        {
            return cma.Remove(action.wa, action.scheduler);
        }

        public ContextMulticastAction Remove(WeakAction wa, AsyncContextRunner runner)
        {
            return new ContextMulticastAction(_actions.Where(a => (a.WeakAction != wa) || (a.ContextRunner != runner)));
        }

        public static ContextMulticastAction operator -(ContextMulticastAction cma, (WeakAction wa, AsyncContextRunner runner) action)
        {
            return cma.Remove(action.wa, action.runner);
        }

        public ContextMulticastAction Remove(WeakAction wa)
        {
            return new ContextMulticastAction(_actions.Where(a => a.WeakAction != wa));
        }

        public static ContextMulticastAction operator -(ContextMulticastAction cma, WeakAction wa)
        {
            return cma.Remove(wa);
        }

        public ContextMulticastAction Remove(Action a, object owner, TaskScheduler scheduler)
        {
            return new ContextMulticastAction(_actions.Where(ac =>
            (ac.Method != a.Method)
            || (ac.WeakAction.Owner != owner)
            || (ac.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastAction operator -(ContextMulticastAction cma, (Action a, object owner, TaskScheduler scheduler) action)
        {
            return cma.Remove(action.a, action.owner, action.scheduler);
        }

        public ContextMulticastAction Remove(object owner, TaskScheduler scheduler)
        {
            return new ContextMulticastAction(_actions.Where(ac =>
            (ac.WeakAction.Owner != owner)
            || (ac.ContextRunner.Scheduler != scheduler)));
        }

        public static ContextMulticastAction operator -(ContextMulticastAction cma, (object owner, TaskScheduler scheduler) action)
        {
            return cma.Remove(action.owner, action.scheduler);
        }

        public ContextMulticastAction Remove(Action a, object owner, AsyncContextRunner runner)
        {
            return new ContextMulticastAction(_actions.Where(ac =>
            (ac.Method != a.Method)
            || (ac.WeakAction.Owner != owner)
            || (ac.ContextRunner != runner)));
        }

        public static ContextMulticastAction operator -(ContextMulticastAction cma, (Action a, object owner, AsyncContextRunner runner) action)
        {
            return cma.Remove(action.a, action.owner, action.runner);
        }

        public ContextMulticastAction Remove(object owner, AsyncContextRunner runner)
        {
            return new ContextMulticastAction(_actions.Where(ac =>
            (ac.WeakAction.Owner != owner)
            || (ac.ContextRunner != runner)));
        }

        public static ContextMulticastAction operator -(ContextMulticastAction cma, (object owner, AsyncContextRunner runner) action)
        {
            return cma.Remove(action.owner, action.runner);
        }

        public ContextMulticastAction Remove(object owner, Action a)
        {
            return new ContextMulticastAction(_actions.Where(ac =>
                (ac.Method != a.Method) ||
                (ac.Owner != owner)));
        }

        public static ContextMulticastAction operator -(ContextMulticastAction cma, (object owner, Action callback) action )
        {
            return cma.Remove(action.owner, action.callback);
        }

        public ContextMulticastAction Remove(object owner)
        {
            return new ContextMulticastAction(_actions.Where(ac => ac.WeakAction.Owner != owner));
        }

        public static ContextMulticastAction operator -(ContextMulticastAction cma, object owner)
        {
            return cma.Remove(owner);
        }


    }
}
