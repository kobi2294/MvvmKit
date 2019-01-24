using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ServiceBase
    {
        private readonly AsyncReaderWriterLock _mutex;
        private readonly AsyncContextRunner _taskFactory;
        private readonly TaskScheduler _scheduler;

        public ServiceBase(TaskScheduler taskScheduler = null)
        {
            _mutex = new AsyncReaderWriterLock();
            _scheduler = taskScheduler ?? TaskScheduler.Default;
            _taskFactory = _scheduler.ToContextRunner();
        }

        private Task<IDisposable> _lock(bool coordinated)
        {
            if (coordinated) return _mutex.WriterLock();
            else return _mutex.ReaderLock();
        }

        protected async Task Run(Action method, bool coordinated = false)
        {
            using (await _lock(coordinated))
            {
                await _taskFactory.Run(method);
            }
        }

        protected async Task<T> Run<T>(Func<T> method, bool coordinated = false)
        {
            using (await _lock(coordinated))
            {
                return await _taskFactory.Run(method);
            }
        }

        protected async Task Run(Func<Task> method, bool coordinated = false)
        {
            using (await _lock(coordinated))
            {
                await _taskFactory.Run(method);
            }
        }

        protected async Task<T> Run<T>(Func<Task<T>> method, bool coordinated = false)
        {
            using (await _lock(coordinated))
            {
                return await _taskFactory.Run(method);
            }
        }
    }
}
