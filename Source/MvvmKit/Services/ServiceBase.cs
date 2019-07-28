using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ServiceBase
    {
        private readonly AsyncContextRunner _runner;
        private readonly TaskScheduler _scheduler;
        private Task _initTask = null;
        private bool _initTaskCalled = false;

        protected virtual Task OnInit()
        {
            return Tasks.Empty;
        }

        private async Task _init()
        {
            await OnInit();
        }

        private Task _ensureInit()
        {
            if (!_initTaskCalled)
            {
                _initTaskCalled = true;
                _initTask = _init();
            }

            return _initTask;
        }

        public Task Init()
        {
            return Run(_ensureInit);
        }

        public ServiceBase(TaskScheduler taskScheduler = null)
        {
            _scheduler = taskScheduler ?? TaskScheduler.Default;
            _runner = _scheduler.ToContextRunner();
        }

        protected Task Run(Action method)
        {
            return _runner.Run(async () =>
            {
                await _ensureInit();
                method();
            });
        }

        protected Task<T> Run<T>(Func<T> func)
        {
            return _runner.Run(async () =>
            {
                await _ensureInit();
                return func();
            });
        }

        protected Task Run(Func<Task> method)
        {
            return _runner.Run(async () =>
            {
                await _ensureInit();
                await method();
            });
        }

        protected Task<T> Run<T>(Func<Task<T>> func)
        {
            return _runner.Run(async () =>
            {
                await _ensureInit();
                return await func();
            });
        }

        public class Runner
        {
            private ServiceBase _owner;

            internal Runner(ServiceBase owner)
            {
                _owner = owner;
            }

            public Task Run(Action method) => _owner.Run(method);

            public Task<T> Run<T>(Func<T> func) => _owner.Run(func);

            public Task Run(Func<Task> method) => _owner.Run(method);

            public Task<T> Run<T>(Func<Task<T>> func) => _owner.Run(func);
        }


        public Runner GetRunner()
        {
            return new Runner(this);
        }
    }
}
