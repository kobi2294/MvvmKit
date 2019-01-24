using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ContextFunc<T, TResult>
    {
        public WeakFunc<T, TResult> WeakFunc { get; }

        public AsyncContextRunner ContextRunner { get; }

        public object Target => WeakFunc.Target;

        public object Owner => WeakFunc.Owner;

        public bool IsAlive => WeakFunc.IsAlive;

        public MethodInfo Method => WeakFunc.Method;


        public ContextFunc(AsyncContextRunner contextRunner, WeakFunc<T, TResult> wa)
        {
            WeakFunc = wa;
            ContextRunner = contextRunner;
        }

        public ContextFunc(TaskScheduler scheduler, WeakFunc<T, TResult> wa)
            :this(scheduler.ToContextRunner(), wa) { }

        public ContextFunc(AsyncContextRunner contextRunner, object owner, Func<T, TResult> action)
            :this(contextRunner, action.ToWeak(owner)) { }

        public ContextFunc(TaskScheduler scheduler, object owner, Func<T, TResult> action)
            : this(scheduler.ToContextRunner(), action.ToWeak(owner)) { }

        public ContextFunc(WeakFunc<T, TResult> wa)
            : this(TaskScheduler.FromCurrentSynchronizationContext(), wa) { }

        public ContextFunc(object owner, Func<T, TResult> a)
            : this(TaskScheduler.FromCurrentSynchronizationContext(), a.ToWeak(owner)) { }

        public Task<TResult> Invoke(T arg)
        {
            return ContextRunner.Run(() => WeakFunc.Execute(arg));
        }

    }
}
