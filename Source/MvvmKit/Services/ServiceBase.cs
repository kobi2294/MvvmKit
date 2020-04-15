using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ServiceBase: BaseDisposable
    {
        private readonly AsyncContextRunner _runner;
        private readonly TaskScheduler _scheduler;
        private AsyncLazyInit _initLazy;
        private AsyncLazyInit _shutDownLazy;


        protected virtual Task OnInit()
        {
            return Tasks.Empty;
        }

        private Task _init()
        {
            return _runner.Run(OnInit);
        }

        public Task Init()
        {
            return _initLazy.Task;
        }

        protected virtual Task OnShutDown()
        {
            return Tasks.Empty;
        }

        private Task _shutDown()
        {
            return _runner.Run(async () =>
            {
                await OnShutDown();
                Dispose();
            });
        }

        public Task ShutDown()
        {
            return _shutDownLazy.Task;
        }

        public ServiceBase(TaskScheduler taskScheduler = null)
        {
            _scheduler = taskScheduler ?? TaskScheduler.Default;
            _runner = _scheduler.ToContextRunner();
            _initLazy = new AsyncLazyInit(_init);
            _shutDownLazy = new AsyncLazyInit(_shutDown);
        }

        protected Task Run(Action method)
        {
            return _runner.Run(() =>
            {
                //await _initLazy.Task;
                method();
            });
        }

        protected Task<T> Run<T>(Func<T> func)
        {
            return _runner.Run(() =>
            {
                //await _initLazy.Task;
                return func();
            });
        }

        protected Task Run(Func<Task> method)
        {
            return _runner.Run(async () =>
            {
                //await _initLazy.Task;
                await method();
            });
        }

        protected Task<T> Run<T>(Func<Task<T>> func)
        {
            return _runner.Run(async () =>
            {
                //await _initLazy.Task;
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
