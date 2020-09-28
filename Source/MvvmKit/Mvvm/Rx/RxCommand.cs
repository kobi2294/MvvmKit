using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmKit
{
    internal class RxCommand<TParam> : BaseDisposable, IRxCommand<TParam>
    {
        private Subject<TParam> _subject = new Subject<TParam>();
        private Func<TParam, bool> _canExecute = p => true;
        private IDisposable _canExecuteSubscription;

        public IRxCommand<TParam> WithCanExecute<TCanExecute>(
            IObservable<TCanExecute> canExecuteObservable,
            Func<TParam, TCanExecute, bool> canExecuteSelector
            )
        {
            _canExecuteSubscription?.Dispose();
            _canExecuteSubscription = canExecuteObservable.Subscribe(val =>
            {
                _canExecute = p => canExecuteSelector(p, val);
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
            return this;
        }

        public IRxCommand<TParam> WithCanExecute(IObservable<bool> canExecuteObservable)
        {
            _canExecuteSubscription?.Dispose();
            _canExecuteSubscription = canExecuteObservable.Subscribe(val =>
            {
                _canExecute = p => val;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
            return this;
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            _canExecuteSubscription?.Dispose();
            _subject.OnCompleted();
        }

        #region ICommand

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            TParam prm = (parameter != null)
                ? (TParam)parameter
                : default;

            var canExecute = _canExecute(prm);
            return canExecute;
        }

        public void Execute(object parameter)
        {
            TParam prm = (parameter != null)
                ? (TParam)parameter
                : default;
            _subject.OnNext(prm);
        }

        #endregion

        #region IObservable

        public IDisposable Subscribe(IObserver<TParam> observer)
        {
            return _subject
                .ObserveOnDispatcher()
                .Subscribe(observer);
        }
        #endregion
    }

    internal class RxCommand : RxCommand<Unit>, IRxCommand
    {
        IRxCommand IRxCommand.WithCanExecute<TCanExecute>(
            IObservable<TCanExecute> canExecuteObservable, 
            Func<TCanExecute, bool> canExecuteSelector)
        {
            return base.WithCanExecute(canExecuteObservable, (u, ce) => canExecuteSelector(ce)) as IRxCommand;
        }

        IRxCommand IRxCommand.WithCanExecute<TCanExecute>(
            IObservable<TCanExecute> canExecuteObservable, 
            Func<Unit, TCanExecute, bool> canExecuteSelector)
        {
            return base.WithCanExecute(canExecuteObservable, canExecuteSelector) as IRxCommand;
        }

        IRxCommand IRxCommand.WithCanExecute(IObservable<bool> canExecuteObservable)
        {
            return base.WithCanExecute(canExecuteObservable) as IRxCommand;
        }
    }
}
