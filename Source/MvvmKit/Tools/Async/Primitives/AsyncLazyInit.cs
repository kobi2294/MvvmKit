using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class AsyncLazyInit
    {
        private readonly object _mutex = new object();

        private readonly Func<Task> _initFunc;

        private readonly Lazy<Task> _lazy;

        private readonly TaskCompletionSource<bool> _completionTask = new TaskCompletionSource<bool>();


        public AsyncLazyInit(Func<Task> initFunc)
        {
            if (initFunc == null)
                throw new ArgumentNullException(nameof(initFunc));

            _initFunc = initFunc;
            _lazy = new Lazy<Task>(_doActualInit);
        }

        private async Task _doActualInit()
        {
            await _initFunc();
            _completionTask.SetResult(true);
        }

        /// <summary>
        /// Ensures the init function has started and returns a task that can be awaited for completion
        /// </summary>
        public Task Ensure() => _lazy.Value;

        /// <summary>
        /// returns a task that can be awaited, but does not eagerly start the init function
        /// </summary>
        public Task CompletionTask => _completionTask.Task;

    }
}
