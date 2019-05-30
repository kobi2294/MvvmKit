using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ContextFunc<TResult>
    {
        public WeakFunc<TResult> WeakFunc { get; }

        public AsyncContextRunner ContextRunner { get; }

        public object Target => WeakFunc.Target;

        public object Owner => WeakFunc.Owner;

        public bool IsAlive => WeakFunc.IsAlive;

        public MethodInfo Method => WeakFunc.Method;


        public ContextFunc(AsyncContextRunner contextRunner, WeakFunc<TResult> wa)
        {
            WeakFunc = wa;
            ContextRunner = contextRunner;
        }

        public ContextFunc(TaskScheduler scheduler, WeakFunc<TResult> wa)
            :this(scheduler.ToContextRunner(), wa) { }

        public ContextFunc(AsyncContextRunner contextRunner, object owner, Func<TResult> action)
            :this(contextRunner, action.ToWeak(owner)) { }

        public ContextFunc(TaskScheduler scheduler, object owner, Func<TResult> action)
            : this(scheduler.ToContextRunner(), action.ToWeak(owner)) { }

        public ContextFunc(WeakFunc<TResult> wa)
            : this(Exec.RunningTaskScheduler, wa) { }

        public ContextFunc(object owner, Func<TResult> a)
            : this(Exec.RunningTaskScheduler, a.ToWeak(owner)) { }


        public Task<TResult> Invoke()
        {
            return ContextRunner.Run(() => WeakFunc.Execute());
        }

    }
}
