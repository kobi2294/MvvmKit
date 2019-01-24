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


        public ContextAction(AsyncContextRunner contextRunner, WeakAction<T1, T2> wa)
        {
            WeakAction = wa;
            ContextRunner = contextRunner;
        }

        public ContextAction(TaskScheduler scheduler, WeakAction<T1, T2> wa)
            : this(scheduler.ToContextRunner(), wa) { }

        public ContextAction(AsyncContextRunner contextRunner, object owner, Action<T1, T2> action)
            : this(contextRunner, action.ToWeak(owner)) { }

        public ContextAction(TaskScheduler scheduler, object owner, Action<T1, T2> action)
            : this(scheduler.ToContextRunner(), action.ToWeak(owner)) { }

        public ContextAction(WeakAction<T1, T2> wa)
            : this(TaskScheduler.FromCurrentSynchronizationContext(), wa) { }

        public ContextAction(object owner, Action<T1, T2> a)
            : this(TaskScheduler.FromCurrentSynchronizationContext(), a.ToWeak(owner)) { }

        public Task Invoke(T1 arg1, T2 arg2)
        {
            return ContextRunner.Run(() => WeakAction.Execute(arg1, arg2));
        }

    }
}
