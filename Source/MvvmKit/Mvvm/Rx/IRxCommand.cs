using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmKit
{
    public interface IRxCommand<T>: ICommand, IDisposable, IObservable<T>
    {
        IRxCommand<T> WithCanExecute<TCanExecute>(IObservable<TCanExecute> canExecuteObservable, Func<T, TCanExecute, bool> canExecuteSelector);

        IRxCommand<T> WithCanExecute(IObservable<bool> canExecuteObservable);
    }

    public interface IRxCommand: IRxCommand<Unit>
    {
        IRxCommand WithCanExecute<TCanExecute>(IObservable<TCanExecute> canExecuteObservable, Func<TCanExecute, bool> canExecuteSelector);

        new IRxCommand WithCanExecute<TCanExecute>(IObservable<TCanExecute> canExecuteObservable, Func<Unit, TCanExecute, bool> canExecuteSelector);

        new IRxCommand WithCanExecute(IObservable<bool> canExecuteObservable);
    }
}
