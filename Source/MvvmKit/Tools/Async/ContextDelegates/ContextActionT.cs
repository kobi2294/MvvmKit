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


        public ContextAction(AsyncContextRunner contextRunner, WeakAction<T> wa)
        {
            WeakAction = wa;
            ContextRunner = contextRunner;
        }

        public ContextAction(TaskScheduler scheduler, WeakAction<T> wa)
            :this(scheduler.ToContextRunner(), wa) { }

        public ContextAction(AsyncContextRunner contextRunner, object owner, Action<T> action)
            :this(contextRunner, action.ToWeak(owner)) { }

        public ContextAction(TaskScheduler scheduler, object owner, Action<T> action)
            : this(scheduler.ToContextRunner(), action.ToWeak(owner)) { }

        public ContextAction(WeakAction<T> wa)
            : this(TaskScheduler.FromCurrentSynchronizationContext(), wa) { }

        public ContextAction(object owner, Action<T> a)
            : this(TaskScheduler.FromCurrentSynchronizationContext(), a.ToWeak(owner)) { }


        public Task Invoke(T arg)
        {
            return ContextRunner.Run(() => WeakAction.Execute(arg));
        }

    }
}
