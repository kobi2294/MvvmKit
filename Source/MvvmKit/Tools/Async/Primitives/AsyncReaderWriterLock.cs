using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class AsyncReaderWriterLock
    {
        private object _mutex;
        private HashSet<IDisposableWithData<LockState>> _currentTokens;
        private Queue<DeferredTask<IDisposable>> _writerQueue;
        private Queue<DeferredTask<IDisposable>> _readerQueue;

        public AsyncReaderWriterLock()
        {
            _mutex = new object();
            _currentTokens = new HashSet<IDisposableWithData<LockState>>();
            _writerQueue = new Queue<DeferredTask<IDisposable>>();
            _readerQueue = new Queue<DeferredTask<IDisposable>>();
        }

        public LockState CurrentState
        {
            get
            {
                return _currentTokens.Any()
                    ? _currentTokens.Max(t => t.Data)
                    : LockState.None;
            }
        }

        public Task<IDisposable> ReaderLock()
        {
            lock (_mutex)
            {
                var token = Disposables.Call(Release, LockState.Read);
                if (CurrentState < LockState.Write)
                {
                    // read or none
                    _currentTokens.Add(token);
                    return token.ToTask<IDisposable>();
                } else
                {
                    var drt = token.ToDeferredTask<IDisposable>();
                    _readerQueue.Enqueue(drt);
                    return drt.Task;
                }
            }
        }

        public Task<IDisposable> WriterLock()
        {
            lock(_mutex)
            {
                var token = Disposables.Call(Release, LockState.Write);
                if(CurrentState == LockState.None)
                {
                    // only if there are no locks at the moment
                    _currentTokens.Add(token);
                    return token.ToTask<IDisposable>();
                } else
                {
                    var drt = token.ToDeferredTask<IDisposable>();
                    _writerQueue.Enqueue(drt);
                    return drt.Task;
                }
            }
        }

        internal void Release(IDisposableWithData<LockState> token)
        {
            lock (_mutex)
            {
                _currentTokens.Remove(token);

                // if there are still locks, then releasing this specific lock has no effect anyway
                if (_currentTokens.Any()) return;

                // give priority to writers
                if (_writerQueue.Any())
                {
                    var drt = _writerQueue.Dequeue();
                    _currentTokens.Add(drt.Result as IDisposableWithData<LockState>);
                    drt.Complete();
                    return;
                }

                // if we got here, then there are only readers waiting
                while (_readerQueue.Any())
                {
                    var drt = _readerQueue.Dequeue();
                    _currentTokens.Add(drt.Result as IDisposableWithData<LockState>);
                    drt.Complete();
                }
            }

        }

    }
}
