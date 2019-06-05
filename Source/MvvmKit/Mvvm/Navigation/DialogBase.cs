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

        public bool ReturnDefaultOnCancel { get; protected set; } = true;
        public T ValueOnCancel { get; protected set; } = default(T);

        #region Commands


        #region Ok Command

        private DelegateCommand<object> _OkCommand;
        public DelegateCommand<object> OkCommand
        {
            get
            {
                if (_OkCommand == null) _OkCommand = new DelegateCommand<object>(OnOkCommand);
                return _OkCommand;
            }
        }

        public void OnOkCommand(object param)
        {
            var val = (T)param;
            SetResult(val);
        }

        #endregion

        #region Cancel Command

        private DelegateCommand _CancelCommand;
        public DelegateCommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null) _CancelCommand = new DelegateCommand(OnCancelCommand);
                return _CancelCommand;
            }
        }

        public void OnCancelCommand()
        {
            SetCanceled();
        }
        #endregion

        #endregion


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
            if (ReturnDefaultOnCancel)
            {
                _taskCompletionSource.SetResult(ValueOnCancel);
            }
            else
            {
                _taskCompletionSource.SetCanceled();
            }
        }

        protected void SetException(Exception exception)
        {
            _taskCompletionSource.SetException(exception);
        }

        protected override Task OnClearing()
        {
            if (!Task.IsCompleted) SetCanceled();
            return base.OnClearing();
        }
    }
}
