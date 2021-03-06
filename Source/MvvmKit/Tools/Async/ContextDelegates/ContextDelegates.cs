﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class ContextDelegates
    {
        #region ActionContext factories

        public static ContextAction InContext(this WeakAction wa, AsyncContextRunner context)
        {
            return new ContextAction(context, wa);
        }

        public static ContextAction InContext(this WeakAction wa, TaskScheduler scheduler)
        {
            return new ContextAction(scheduler, wa);
        }

        public static ContextAction InContext(this WeakAction wa)
        {
            return new ContextAction(wa);
        }

        public static ContextAction InContext(this Action a, object owner, AsyncContextRunner context)
        {
            return new ContextAction(context, owner, a);
        }

        public static ContextAction InContext(this Action a, object owner, TaskScheduler scheduler)
        {
            return new ContextAction(scheduler, owner, a);
        }

        public static ContextAction InContext(this Action a, object owner)
        {
            return new ContextAction(owner, a);
        }

        #endregion

        #region ContextAction<T> Factories

        public static ContextAction<T> InContext<T>(this WeakAction<T> wa, AsyncContextRunner context)
        {
            return new ContextAction<T>(context, wa);
        }

        public static ContextAction<T> InContext<T>(this WeakAction<T> wa, TaskScheduler scheduler)
        {
            return new ContextAction<T>(scheduler, wa);
        }

        public static ContextAction<T> InContext<T>(this WeakAction<T> wa)
        {
            return new ContextAction<T>(wa);
        }

        public static ContextAction<T> InContext<T>(this Action<T> a, object owner, AsyncContextRunner context)
        {
            return new ContextAction<T>(context, owner, a);
        }

        public static ContextAction<T> InContext<T>(this Action<T> a, object owner, TaskScheduler scheduler)
        {
            return new ContextAction<T>(scheduler, owner, a);
        }

        public static ContextAction<T> InContext<T>(this Action<T> a, object owner)
        {
            return new ContextAction<T>(owner, a);
        }

        #endregion

        #region ContextAction<T1, T2> Factories

        public static ContextAction<T1, T2> InContext<T1, T2>(this WeakAction<T1, T2> wa, AsyncContextRunner context)
        {
            return new ContextAction<T1, T2>(context, wa);
        }

        public static ContextAction<T1, T2> InContext<T1, T2>(this WeakAction<T1, T2> wa, TaskScheduler scheduler)
        {
            return new ContextAction<T1, T2>(scheduler, wa);
        }

        public static ContextAction<T1, T2> InContext<T1, T2>(this WeakAction<T1, T2> wa)
        {
            return new ContextAction<T1, T2>(wa);
        }

        public static ContextAction<T1, T2> InContext<T1, T2>(this Action<T1, T2> a, object owner, AsyncContextRunner context)
        {
            return new ContextAction<T1, T2>(context, owner, a);
        }

        public static ContextAction<T1, T2> InContext<T1, T2>(this Action<T1, T2> a, object owner, TaskScheduler scheduler)
        {
            return new ContextAction<T1, T2>(scheduler, owner, a);
        }

        public static ContextAction<T1, T2> InContext<T1, T2>(this Action<T1, T2> a, object owner)
        {
            return new ContextAction<T1, T2>(owner, a);
        }

        #endregion

        #region ContextFunc<TRes> Factories

        public static ContextFunc<TRes> InContext<TRes>(this WeakFunc<TRes> wa, AsyncContextRunner context)
        {
            return new ContextFunc<TRes>(context, wa);
        }

        public static ContextFunc<TRes> InContext<TRes>(this WeakFunc<TRes> wa, TaskScheduler scheduler)
        {
            return new ContextFunc<TRes>(scheduler, wa);
        }

        public static ContextFunc<TRes> InContext<TRes>(this WeakFunc<TRes> wa)
        {
            return new ContextFunc<TRes>(wa);
        }

        public static ContextFunc<TRes> InContext<TRes>(this Func<TRes> a, AsyncContextRunner context, object owner)
        {
            return new ContextFunc<TRes>(context, owner, a);
        }

        public static ContextFunc<TRes> InContext<TRes>(this Func<TRes> a, TaskScheduler scheduler, object owner)
        {
            return new ContextFunc<TRes>(scheduler, owner, a);
        }

        public static ContextFunc<TRes> InContext<TRes>(this Func<TRes> a, object owner)
        {
            return new ContextFunc<TRes>(owner, a);
        }

        #endregion

        #region ContextFunc<T, TRes> Factories

        public static ContextFunc<T, TRes> InContext<T, TRes>(this WeakFunc<T, TRes> wa, AsyncContextRunner context)
        {
            return new ContextFunc<T, TRes>(context, wa);
        }

        public static ContextFunc<T, TRes> InContext<T, TRes>(this WeakFunc<T, TRes> wa, TaskScheduler scheduler)
        {
            return new ContextFunc<T, TRes>(scheduler, wa);
        }

        public static ContextFunc<T, TRes> InContext<T, TRes>(this WeakFunc<T, TRes> wa)
        {
            return new ContextFunc<T, TRes>(wa);
        }

        public static ContextFunc<T, TRes> InContext<T, TRes>(this Func<T, TRes> a, AsyncContextRunner context, object owner)
        {
            return new ContextFunc<T, TRes>(context, owner, a);
        }

        public static ContextFunc<T, TRes> InContext<T, TRes>(this Func<T, TRes> a, TaskScheduler scheduler, object owner)
        {
            return new ContextFunc<T, TRes>(scheduler, owner, a);
        }

        public static ContextFunc<T, TRes> InContext<T, TRes>(this Func<T, TRes> a, object owner)
        {
            return new ContextFunc<T, TRes>(owner, a);
        }

        #endregion

    }
}
