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


        public ContextAction(AsyncContextRunner contextRunner, WeakAction wa)
        {
            WeakAction = wa;
            ContextRunner = contextRunner;
        }

        public ContextAction(TaskScheduler scheduler, WeakAction wa)
            :this(scheduler.ToContextRunner(), wa) { }

        public ContextAction(AsyncContextRunner contextRunner, object owner, Action action)
            :this(contextRunner, action.ToWeak(owner)) { }

        public ContextAction(TaskScheduler scheduler, object owner, Action action)
            : this(scheduler.ToContextRunner(), action.ToWeak(owner)) { }

        public ContextAction(WeakAction wa)
            : this(TaskScheduler.FromCurrentSynchronizationContext(), wa) { }

        public ContextAction(object owner, Action a)
            : this(TaskScheduler.FromCurrentSynchronizationContext(), a.ToWeak(owner)) { }

        public Task Invoke()
        {
            return ContextRunner.Run(() => WeakAction.Execute());
        }

    }
}
