using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class AsyncEvent
    {
        private ContextMulticastFuncTask _handlers;
        private object _mutex = new object();

        public Task Subscribe(object owner, Func<Task> callback)
        {
            lock (_mutex)
            {
                _handlers += (owner, callback);
            }
            return callback();
        }

        public Task Unsubscribe(object owner, Func<Task> callback = null)
        {
            if (callback == null)
            {
                lock (_mutex)
                {
                    _handlers -= owner;
                }
            } else
            {
                lock(_mutex)
                {
                    _handlers -= (owner, callback);
                }
            }
            return Tasks.Empty;
        }

        public Task Invoke()
        {
            return _handlers.Invoke();
        }

        public AsyncEvent()
        {
            _handlers = new ContextMulticastFuncTask();
        }
    }
}
