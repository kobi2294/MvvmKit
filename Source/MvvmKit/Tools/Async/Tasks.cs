using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class Tasks
    {
        public static Task Empty => Task.CompletedTask;

        public static Task<bool> True => Task.FromResult(true);

        public static Task<bool> False => Task.FromResult(false);

        public static Task<T> ToTask<T>(this T value)
        {
            return Task.FromResult(value);
        }

        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks)
        {
            return Task.WhenAll(tasks);
        }

        public static DeferredTask<T> ToDeferredTask<T>(this T value)
        {
            return new DeferredTask<T>(value);
        }

        public static AsyncContextRunner ToContextRunner(this TaskScheduler scheduler)
        {
            return AsyncContextRunner.For(scheduler);
        }
    }
}
