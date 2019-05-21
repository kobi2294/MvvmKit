using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class DeferredTask<T>
    {
        private TaskCompletionSource<T> _tcs;
        
        public T Result { get; }

        public Task<T> Task => _tcs.Task;

        public DeferredTask(T result)
        {
            _tcs = new TaskCompletionSource<T>();
            Result = result;
        }

        public void Complete()
        {
            _tcs.SetResult(Result);
        }
    }
}
