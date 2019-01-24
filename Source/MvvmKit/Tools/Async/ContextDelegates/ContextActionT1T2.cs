using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ContextAction<T1, T2>
    {
        public WeakAction<T1, T2> WeakAction { get; }

        public AsyncContextRunner ContextRunner { get; }

        public object Target => WeakAction.Target;

        public bool IsAlive => WeakAction.IsAlive;

        public MethodInfo Method => WeakAction.Method;


        public ContextAction(WeakAction<T1, T2> wa, AsyncContextRunner contextRunner)
        {
            WeakAction = wa;
            ContextRunner = contextRunner;
        }

        public ContextAction(WeakAction<T1, T2> wa, TaskScheduler scheduler)
            : this(wa, scheduler.ToContextRunner()) { }

        public ContextAction(Action<T1, T2> action, object owner, AsyncContextRunner contextRunner)
            : this(action.ToWeak(owner), contextRunner) { }

        public ContextAction(Action<T1, T2> action, object owner, TaskScheduler scheduler)
            : this(action.ToWeak(owner), scheduler.ToContextRunner()) { }

        public ContextAction(WeakAction<T1, T2> wa)
            : this(wa, TaskScheduler.FromCurrentSynchronizationContext()) { }

        public ContextAction(Action<T1, T2> a, object owner)
            : this(a.ToWeak(owner), TaskScheduler.FromCurrentSynchronizationContext()) { }

        public Task Invoke(T1 arg1, T2 arg2)
        {
            return ContextRunner.Run(() => WeakAction.Execute(arg1, arg2));
        }

    }
}
