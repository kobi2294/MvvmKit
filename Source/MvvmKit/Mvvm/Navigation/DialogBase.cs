﻿using System;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class DialogBase : ComponentBase
    {
        private TaskCompletionSource<object> _taskCompletionSource;
        public bool ReturnDefaultOnCancel { get; protected set; } = true;

        #region Ok Command

        private DelegateCommand _OkCommand;
        public virtual DelegateCommand OkCommand
        {
            get
            {
                if (_OkCommand == null) _OkCommand = new DelegateCommand(OnOkCommand);
                return _OkCommand;
            }
        }

        public void OnOkCommand()
        {
            SetResult();
        }

        #endregion

        #region Cancel Command

        private DelegateCommand _CancelCommand;
        public virtual DelegateCommand CancelCommand
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

        public Task Task => _taskCompletionSource.Task;

        public DialogBase()
        {
            _taskCompletionSource = new TaskCompletionSource<object>();
        }

        protected void SetResult()
        {
            _taskCompletionSource.SetResult(null);
        }

        protected void SetCanceled()
        {
            if (ReturnDefaultOnCancel)
            {
                _taskCompletionSource.SetResult(null);
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

    public abstract class DialogBase<T> : ComponentBase
    {
        private TaskCompletionSource<T> _taskCompletionSource;
        public bool ReturnDefaultOnCancel { get; protected set; } = true;


        public T ValueOnCancel { get; protected set; } = default(T);

        #region Commands


        #region Ok Command

        private DelegateCommand<object> _OkCommand;
        public virtual DelegateCommand<object> OkCommand
        {
            get
            {
                if (_OkCommand == null) _OkCommand = new DelegateCommand<object>(OnOkCommand);
                return _OkCommand;
            }
        }

        public void OnOkCommand(object param)
        {
            if (param is T value)
                SetResult(value);
            else
                SetResult(default(T));
        }

        #endregion

        #region Cancel Command

        private DelegateCommand _CancelCommand;
        public virtual DelegateCommand CancelCommand
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
