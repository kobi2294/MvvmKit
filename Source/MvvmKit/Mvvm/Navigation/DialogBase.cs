using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class DialogBase<T> : ComponentBase
    {
        private TaskCompletionSource<T> _taskCompletionSource;

        internal bool AllowCancellation { get; set; }

        public Task<T> Task => _taskCompletionSource.Task;

        public DialogBase()
        {
            _taskCompletionSource = new TaskCompletionSource<T>();
        }

        protected void SetResult(T result)
        {
            _taskCompletionSource.SetResult(result);
        }

        protected void SetCanceled()
        {
            if (AllowCancellation)
            {
                _taskCompletionSource.SetCanceled();
            }
            else
            {
                _taskCompletionSource.SetResult(default(T));
            }
        }

        protected void SetException(Exception exception)
        {
            _taskCompletionSource.SetException(exception);
        }

        protected override Task OnBeforeDectivated()
        {
            if (!Task.IsCompleted) SetCanceled();
            return base.OnBeforeDectivated();
        }
    }
}
