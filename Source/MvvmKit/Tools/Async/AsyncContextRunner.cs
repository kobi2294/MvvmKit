using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class AsyncContextRunner
    {
        #region Static repository

        private static object _mutex = new object();
        private static Dictionary<TaskScheduler, AsyncContextRunner> _knownRunners = new Dictionary<TaskScheduler, AsyncContextRunner>();

        public static AsyncContextRunner For(TaskScheduler scheduler)
        {
            lock(_mutex)
            {
                if (!_knownRunners.ContainsKey(scheduler))
                {
                    _knownRunners.Add(scheduler, new AsyncContextRunner(scheduler));
                }
                return _knownRunners[scheduler];
            }
        }

        #endregion

        private TaskFactory _factory;

        public TaskScheduler Scheduler => _factory.Scheduler;

        private AsyncContextRunner(TaskScheduler scheduler)
        {
            _factory = new TaskFactory(CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                           TaskContinuationOptions.None, scheduler);
        }

        public Task Run(Func<Task> func)
        {
            if (Exec.RunningTaskScheduler == Scheduler)
            {
                return func();
            } else
            {
                return _factory.StartNew(func).Unwrap();
            }
        }

        public Task<T> Run<T>(Func<Task<T>> func)
        {
            if (Exec.RunningTaskScheduler == Scheduler)
            {
                return func();
            }
            else
            {
                return _factory.StartNew(func).Unwrap();
            }
        }

        public Task Run(Action action)
        {
            if (Exec.RunningTaskScheduler == Scheduler)
            {
                action();
                return Task.CompletedTask;
            }
            else
            {
                return _factory.StartNew(action);
            }
        }

        public Task<T> Run<T>(Func<T> func)
        {
            if (Exec.RunningTaskScheduler == Scheduler)
            {
                var res = func();
                return Task.FromResult(res);
            }
            else
            {
                return _factory.StartNew(func);
            }
        }

    }
}
