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

        private static Dictionary<TaskScheduler, AsyncContextRunner> _knownFactories = new Dictionary<TaskScheduler, AsyncContextRunner>();

        public static AsyncContextRunner For(TaskScheduler scheduler)
        {
            if (!_knownFactories.ContainsKey(scheduler))
            {
                _knownFactories.Add(scheduler, new AsyncContextRunner(scheduler));
            }
            return _knownFactories[scheduler];
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
            return _factory.StartNew(func).Unwrap();
        }

        public Task<T> Run<T>(Func<Task<T>> func)
        {
            return _factory.StartNew(func).Unwrap();
        }

        public Task Run(Action action)
        {
            return _factory.StartNew(action);
        }

        public Task<T> Run<T>(Func<T> func)
        {
            return _factory.StartNew(func);
        }

    }
}
