using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    /// <summary>
    /// Async Mutex is a primitive that allows to make an "async" lock on a resource.
    /// </summary>
    /// <example>
    /// This sample shows how to use the class 
    /// <code>
    /// private AsyncMutex _mutex = new AsyncMutex();
    /// 
    /// public async Task DoSensetiveWork() {
    ///     using(await _mutex.Lock()) 
    ///     {
    ///         await _useResource();
    ///     }
    /// }
    /// </code>
    /// </example>
    /// 

    public class AsyncMutex
    {
        private IDisposable _currentToken;
        private Queue<DeferredTask<IDisposable>> _requests;
        private object _mutex;

        public AsyncMutex()
        {
            _currentToken = null;
            _requests = new Queue<DeferredTask<IDisposable>>();
            _mutex = new object();
        }

        public Task<IDisposable> Lock()
        {
            //if (we are not locked) 
            //{
            //    return Task that is already finished
            //} else {
            //    create task completion source, so we can return the token later
            //        and return the task
            //}

            lock (_mutex)
            {
                var token = Disposables.Call(Release);

                if (_currentToken == null)
                {
                    _currentToken = token;
                    return _currentToken.ToTask();
                }
                else
                {
                    var drt = token.ToDeferredTask();
                    _requests.Enqueue(drt);
                    return drt.Task;
                }
            }
        }

        internal void Release()
        {
            //if there are pending reuqests, 
            //take the next request task and complete it with a new token

            lock (_mutex)
            {
                if (_requests.Any())
                {
                    var drt = _requests.Dequeue();
                    _currentToken = drt.Result;
                    drt.Complete();
                }
                else
                {
                    _currentToken = null;
                }
            }
        }
    }
}
