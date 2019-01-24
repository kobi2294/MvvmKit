using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ContextAction
    {
        public WeakAction WeakAction { get; }

        public AsyncContextRunner ContextRunner { get; }

        public object Target => WeakAction.Target;

        public object Owner => WeakAction.Owner;

        public bool IsAlive => WeakAction.IsAlive;

        public MethodInfo Method => WeakAction.Method;


        public ContextAction(WeakAction wa, AsyncContextRunner contextRunner)
        {
            WeakAction = wa;
            ContextRunner = contextRunner;
        }

        public ContextAction(WeakAction wa, TaskScheduler scheduler)
            :this(wa, scheduler.ToContextRunner()) { }

        public ContextAction(Action action, object owner, AsyncContextRunner contextRunner)
            :this(action.ToWeak(owner), contextRunner) { }

        public ContextAction(Action action, object owner, TaskScheduler scheduler)
            : this(action.ToWeak(owner), scheduler.ToContextRunner()) { }

        public ContextAction(WeakAction wa)
            : this(wa, TaskScheduler.FromCurrentSynchronizationContext()) { }

        public ContextAction(Action a, object owner)
            : this(a.ToWeak(owner), TaskScheduler.FromCurrentSynchronizationContext()) { }

        public Task Invoke()
        {
            return ContextRunner.Run(() => WeakAction.Execute());
        }

    }
}
