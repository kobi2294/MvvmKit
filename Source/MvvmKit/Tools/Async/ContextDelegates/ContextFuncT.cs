﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ContextFunc<T, TResult>
    {
        private readonly WeakFunc<T, TResult> WeakFunc;

        public AsyncContextRunner ContextRunner { get; }

        public object Target => WeakFunc.Target;

        public object Owner => WeakFunc.Owner;

        public bool IsAlive => WeakFunc.IsAlive;

        public MethodInfo Method => WeakFunc.Method;


        public ContextFunc(WeakFunc<T, TResult> wa, AsyncContextRunner contextRunner)
        {
            WeakFunc = wa;
            ContextRunner = contextRunner;
        }

        public ContextFunc(WeakFunc<T, TResult> wa, TaskScheduler scheduler)
            :this(wa, scheduler.ToContextRunner()) { }

        public ContextFunc(Func<T, TResult> action, object owner, AsyncContextRunner contextRunner)
            :this(action.ToWeak(owner), contextRunner) { }

        public ContextFunc(Func<T, TResult> action, object owner, TaskScheduler scheduler)
            : this(action.ToWeak(owner), scheduler.ToContextRunner()) { }

        public ContextFunc(WeakFunc<T, TResult> wa)
            : this(wa, TaskScheduler.FromCurrentSynchronizationContext()) { }

        public ContextFunc(Func<T, TResult> a, object owner)
            : this(a.ToWeak(owner), TaskScheduler.FromCurrentSynchronizationContext()) { }

        public Task<TResult> Invoke(T arg)
        {
            return ContextRunner.Run(() => WeakFunc.Execute(arg));
        }

    }
}
