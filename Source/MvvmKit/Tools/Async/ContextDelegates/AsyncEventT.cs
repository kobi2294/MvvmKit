using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class AsyncEvent<T>
    {
        private ContextMulticastFuncTask<T> _handlers;

        public T LatestValue { get; private set; }

        public Task Subscribe(object owner, Func<T, Task> callback)
        {
            _handlers += (owner, callback);
            return callback(LatestValue);
        }

        public Task Subscribe(object onString)
        {
            throw new NotImplementedException();
        }

        public Task Unsubscribe(object owner, Func<T, Task> callback = null)
        {
            if (callback == null)
            {
                _handlers -= owner;
            }
            else
            {
                _handlers -= (owner, callback);
            }
            return Tasks.Empty;
        }

        public Task Invoke(T value)
        {
            LatestValue = value;
            return _handlers.Invoke(value);
        }


        public AsyncEvent(T initValue = default(T))
        {
            _handlers = new ContextMulticastFuncTask<T>();
            LatestValue = initValue;
        }

    }
}
