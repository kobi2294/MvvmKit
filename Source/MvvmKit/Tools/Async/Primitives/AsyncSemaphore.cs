using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class AsyncSemaphore
    {
        private object _mutex;
        private HashSet<INotifyDisposable> _currentTokens;
        private Queue<DeferredTask<INotifyDisposable>> _queue;

        public int LocksLimit { get; private set; }

        public int LocksCount
        {
            get
            {
                return _currentTokens.Count;
            }
        }


        public AsyncSemaphore(int locksLimit)
        {
            LocksLimit = locksLimit;
            _mutex = new object();
            _currentTokens = new HashSet<INotifyDisposable>();
            _queue = new Queue<DeferredTask<INotifyDisposable>>();
        }

        public Task<INotifyDisposable> Lock()
        {
            lock(_mutex)
            {
                var token = Disposables.Call(Release);
                if (LocksCount < LocksLimit)
                {
                    _currentTokens.Add(token);
                    return token.ToTask();
                } else
                {
                    var drt = token.ToDeferredTask();
                    _queue.Enqueue(drt);
                    return drt.Task;
                }
            }
        }

        public void Release(IDisposable token)
        {
            lock (_mutex)
            {
                _currentTokens.Remove((INotifyDisposable)token);

                while ((_queue.Any()) && (LocksCount < LocksLimit))
                {
                    var drt = _queue.Dequeue();
                    _currentTokens.Add(drt.Result);
                    drt.Complete();   
                }
            }
        }
    }
}
