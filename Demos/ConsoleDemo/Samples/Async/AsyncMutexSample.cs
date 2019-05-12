using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Async
{
    public static class AsyncMutexSample
    {
        private static object _mutex = new object();
        private static AsyncMutex _asyncMutex = new AsyncMutex();

        public static async Task RunAsync()
        {
            using (await _asyncMutex.Lock())
            {
                _commonData.Add(42);
            }

        }

        private static AsyncSemaphore _semaphore = new AsyncSemaphore(5);

        public static void Run()
        {
            lock(_mutex)
            {
                // access common data
                _commonData.Add(42);
            }
        }

        private static List<int> _commonData = new List<int>();
        private static AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();

        public async static Task<int> ReadFirstItem()
        {
            using (await _lock.ReaderLock()) {
                return _commonData.First();
            }
        }

        public async static  Task UpdateItem()
        {
            using (await _lock.WriterLock())
            {
                _commonData.Add(42);
            }
        }

    }
}
