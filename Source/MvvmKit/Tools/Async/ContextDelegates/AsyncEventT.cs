using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class AsyncEvent<T>: IAsyncEventWithData
    {
        private ContextMulticastFuncTask<T> _handlers;

        public T LatestValue { get; private set; }

        private Func<Func<T, Task>, Task> _onSubscribe;

        public Task Subscribe(object owner, Func<T, Task> callback)
        {
            _handlers += (owner, callback);
            return _onSubscribe(callback);
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

        Task IAsyncEventWithData.Invoke(object value)
        {
            return Invoke((T)value);
        }


        public AsyncEvent<T> OnSubscribe(Func<Func<T, Task>, Task> onSubscribe)
        {
            _onSubscribe = onSubscribe;
            return this;
        }

        public AsyncEvent(T initValue = default(T))
        {
            _handlers = new ContextMulticastFuncTask<T>();

            // by default, on subscribe we notify with the latest value
            _onSubscribe = async (cb) =>
            {
                await cb(LatestValue);
            };

            LatestValue = initValue;
        }

    }
}
