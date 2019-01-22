using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class Weaks
    {
        public static WeakAction ToWeak(this Action action, object observer)
        {
            return new WeakAction(observer, action);
        }

        public static WeakAction<T> ToWeak<T>(this Action<T> action, object observer)
        {
            return new WeakAction<T>(observer, action);
        }

        public static WeakAction<T1, T2> ToWeak<T1, T2>(this Action<T1, T2> action, object observer)
        {
            return new WeakAction<T1, T2>(observer, action);
        }

        public static WeakFunc<TResult> ToWeak<TResult>(this Func<TResult> func, object observer)
        {
            return new WeakFunc<TResult>(observer, func);
        }

        public static WeakFunc<T, TResult> ToWeak<T, TResult>(this Func<T, TResult> func, object observer)
        {
            return new WeakFunc<T, TResult>(observer, func);
        }
    }
}
