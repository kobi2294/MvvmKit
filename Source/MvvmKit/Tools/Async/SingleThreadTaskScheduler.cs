using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class SingleThreadTaskScheduler : TaskScheduler, IDisposable
    {
        // the single thread that runs all tasks
        private readonly Thread _thread;

        // we use a cancellation token to stop the thread and exit the main loop
        private readonly CancellationTokenSource _cancellationToken;

        // we use a concurrent collection to allow multiple threads to add tasks in a thread safe manner.
        private readonly BlockingCollection<Task> _tasks;

        public SingleThreadTaskScheduler(string name = "Single Thread Scheduler")
        {
            _cancellationToken = new CancellationTokenSource();
            _tasks = new BlockingCollection<Task>();

            _thread = new Thread(_threadStart) { Name = name };
            _thread.IsBackground = true;
            _thread.Start();
        }

        private void _threadStart()
        {
            try
            {
                var token = _cancellationToken.Token;

                // "consume" the next items from the collection and execute them one by one.
                // note that the only way to get out of this loop is by invoking the cancellation token, or if the collection is completed
                // which will throw a cancellation exception and will take us to the finally clause
                foreach (var task in _tasks.GetConsumingEnumerable(token))
                {
                    TryExecuteTask(task);
                }
            }
            finally
            {
                _tasks.Dispose();
            }
        }

        private void _verifyNotDisposed()
        {
            if (_cancellationToken.IsCancellationRequested)
                throw new ObjectDisposedException(nameof(SingleThreadTaskScheduler));
        }

        public override int MaximumConcurrencyLevel => 1;

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            _verifyNotDisposed();

            // we duplicate the collection of course, to avoid enumeration problems
            return _tasks.ToArray();
        }

        protected override void QueueTask(Task task)
        {
            // we can't add items to the collection once we have been disposed of...
            _verifyNotDisposed();

            // just add them to the collection
            _tasks.Add(task, this._cancellationToken.Token);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            _verifyNotDisposed();

            // make sure this is running in the currect thread
            if (_thread != Thread.CurrentThread)
                return false;

            // make sure we are not disposed of
            if (this._cancellationToken.Token.IsCancellationRequested)
                return false;

            TryExecuteTask(task);
            return true;
        }

        public void Dispose()
        {
            // cancellation happens as part of the Dispose method only, so if the cancellation
            // was already requested, it means this is not the first time Dispose() is called
            if (_cancellationToken.IsCancellationRequested)
                return;

            // Complete the blocking collection
            _tasks.CompleteAdding();

            // invoke the cancellation token
            _cancellationToken.Cancel();
        }
    }
}
