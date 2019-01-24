using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class Weaks
    {
        /// <summary>
        /// Creates an Instance of WeakAction for the action
        /// </summary>
        /// <param name="action">The delegate</param>
        /// <param name="owner">An object that serves as lifetime controller - As long as it is "alive" the WeakAction remains alive</param>
        /// <returns>WeakAction instance of the action</returns>
        public static WeakAction ToWeak(this Action action, object owner)
        {
            return new WeakAction(owner, action);
        }


        /// <summary>
        /// Creates an Instance of WeakAction for the action
        /// </summary>
        /// <typeparam name="T">The Action Type argument</typeparam>
        /// <param name="action">The delegate</param>
        /// <param name="owner">An object that serves as lifetime controller - As long as it is "alive" the WeakAction remains alive</param>
        /// <returns>WeakAction instance of the action</returns>
        public static WeakAction<T> ToWeak<T>(this Action<T> action, object owner)
        {
            return new WeakAction<T>(owner, action);
        }

        /// <summary>
        /// Creates an Instance of WeakAction for the action
        /// </summary>
        /// <typeparam name="T1">Action Type Argument 1</typeparam>
        /// <typeparam name="T2">Action Type Argument 2</typeparam>
        /// <param name="action">The delegate</param>
        /// <param name="owner">An object that serves as lifetime controller - As long as it is "alive" the WeakAction remains alive</param>
        /// <returns>WeakAction instance of the action</returns>
        public static WeakAction<T1, T2> ToWeak<T1, T2>(this Action<T1, T2> action, object owner)
        {
            return new WeakAction<T1, T2>(owner, action);
        }

        /// <summary>
        /// Creates an Instance of WeakFunc for the function
        /// </summary>
        /// <typeparam name="TResult">Function result type</typeparam>
        /// <param name="func">The delegate</param>
        /// <param name="owner">An object that serves as lifetime controller - As long as it is "alive" the WeakFunc remains alive</param>
        /// <returns>WeakFunc instance of the function</returns>
        public static WeakFunc<TResult> ToWeak<TResult>(this Func<TResult> func, object owner)
        {
            return new WeakFunc<TResult>(owner, func);
        }

        /// <summary>
        /// Creates an Instance of WeakFunc for the function
        /// </summary>
        /// <typeparam name="T">Function argument type</typeparam>
        /// <typeparam name="TResult">Function result type</typeparam>
        /// <param name="func">The delegate</param>
        /// <param name="owner">An object that serves as lifetime controller - As long as it is "alive" the WeakFunc remains alive</param>
        /// <returns>WeakFunc instance of the function</returns>
        public static WeakFunc<T, TResult> ToWeak<T, TResult>(this Func<T, TResult> func, object owner)
        {
            return new WeakFunc<T, TResult>(owner, func);
        }
    }
}
