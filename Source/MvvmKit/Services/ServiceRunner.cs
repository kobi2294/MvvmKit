using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ServiceRunner
    {
        private AsyncReaderWriterLock _mutex;
        private AsyncContextRunner _taskFactory;

        internal ServiceRunner(AsyncReaderWriterLock mutex, AsyncContextRunner taskFactory)
        {
            _mutex = mutex;
            _taskFactory = taskFactory;
        }
        private Task<IDisposable> _lock(bool coordinated)
        {
            if (coordinated) return _mutex.WriterLock();
            else return _mutex.ReaderLock();
        }

        public async Task Run(Action method, bool coordinated = false)
        {
            using (await _lock(coordinated))
            {
                await _taskFactory.Run(method);
            }
        }

        public async Task<T> Run<T>(Func<T> method, bool coordinated = false)
        {
            using (await _lock(coordinated))
            {
                return await _taskFactory.Run(method);
            }
        }

        public async Task Run(Func<Task> method, bool coordinated = false)
        {
            using (await _lock(coordinated))
            {
                await _taskFactory.Run(method);
            }
        }

        public async Task<T> Run<T>(Func<Task<T>> method, bool coordinated = false)
        {
            using (await _lock(coordinated))
            {
                return await _taskFactory.Run(method);
            }
        }

    }
}
