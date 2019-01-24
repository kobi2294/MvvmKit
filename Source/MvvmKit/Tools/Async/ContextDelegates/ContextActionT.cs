using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ContextAction<T>
    {
        public WeakAction<T> WeakAction { get; }

        public AsyncContextRunner ContextRunner { get; }

        public object Target => WeakAction.Target;

        public object Owner => WeakAction.Owner;

        public bool IsAlive => WeakAction.IsAlive;

        public MethodInfo Method => WeakAction.Method;


        public ContextAction(WeakAction<T> wa, AsyncContextRunner contextRunner)
        {
            WeakAction = wa;
            ContextRunner = contextRunner;
        }

        public ContextAction(WeakAction<T> wa, TaskScheduler scheduler)
            :this(wa, scheduler.ToContextRunner()) { }

        public ContextAction(Action<T> action, object owner, AsyncContextRunner contextRunner)
            :this(action.ToWeak(owner), contextRunner) { }

        public ContextAction(Action<T> action, object owner, TaskScheduler scheduler)
            : this(action.ToWeak(owner), scheduler.ToContextRunner()) { }

        public ContextAction(WeakAction<T> wa)
            : this(wa, TaskScheduler.FromCurrentSynchronizationContext()) { }

        public ContextAction(Action<T> a, object owner)
            : this(a.ToWeak(owner), TaskScheduler.FromCurrentSynchronizationContext()) { }


        public Task Invoke(T arg)
        {
            return ContextRunner.Run(() => WeakAction.Execute(arg));
        }

    }
}
