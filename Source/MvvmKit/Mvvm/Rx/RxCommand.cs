using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmKit
{
    internal class RxCommand<TParam, TCanExecute> : BaseDisposable, IRxCommand<TParam>
    {
        private Subject<TParam> _subject = new Subject<TParam>();
        private Func<TParam, TCanExecute, bool> _canExecuteFunc = (res, cx) => true;
        private TCanExecute _latestCanExecuteValue = default(TCanExecute);
        private IObservable<TCanExecute> _canExecuteObservable = Observable.Return<TCanExecute>(default(TCanExecute));

        internal RxCommand(
            IObservable<TCanExecute> canExecuteObservable = null, 
            Func<TParam, TCanExecute, bool> canExecuteFunc = null
            )
        {
            if (canExecuteObservable != null) _canExecuteObservable = canExecuteObservable;
            if (canExecuteFunc != null) _canExecuteFunc = canExecuteFunc;

            _canExecuteObservable.Subscribe(val =>
            {
                _latestCanExecuteValue = val;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }).DisposedBy(this);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            _subject.Dispose();
        }

        #region ICommand

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var res = (TParam)parameter;
            var canExecute = _canExecuteFunc(res, _latestCanExecuteValue);
            return canExecute;
        }

        public void Execute(object parameter)
        {
            var res = (TParam)parameter;
            _subject.OnNext(res);
        }

        #endregion

        #region IObservable

        public IDisposable Subscribe(IObserver<TParam> observer)
        {
            return _subject.Subscribe(observer);
        }
        #endregion
    }

    internal class RxCommand<TCanExecute>: RxCommand<object, TCanExecute>, IRxCommand
    {
        internal RxCommand(
            IObservable<TCanExecute> canExecuteObservable = null,
            Func<TCanExecute, bool> canExecuteFunc = null
            )
            :base(
                 canExecuteObservable, 
                 (obj, cx) => canExecuteFunc != null ? canExecuteFunc(cx) : true
                 )
        {
        }
    }

    internal class RxCommand: RxCommand<object>
    {
        internal RxCommand()
            :base(null, null)
        { }
    }
}
