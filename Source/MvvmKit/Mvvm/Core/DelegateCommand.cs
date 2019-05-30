using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class DelegateCommand : Prism.Commands.PrismDelegateCommand
    {
        /// <summary>
        /// Creates a new instance of <see cref="DelegateCommand"/> with the <see cref="Action"/> to invoke on execution.
        /// </summary>
        /// <param name="execute">The <see cref="Action"/> to invoke when <see cref="ICommand.Execute"/> is called.</param>
        public DelegateCommand(Action execute)
            :base(execute)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DelegateCommand"/> with the <see cref="Action"/> to invoke on execution
        /// and a <see langword="Func" /> to query for determining if the command can execute.
        /// </summary>
        /// <param name="execute">The <see cref="Action"/> to invoke when <see cref="ICommand.Execute"/> is called.</param>
        /// <param name="canExecute">The <see cref="Func{TResult}"/> to invoke when <see cref="ICommand.CanExecute"/> is called</param>
        public DelegateCommand(Action execute, Func<bool> canExecute)
            : base(execute, canExecute)
        {
        }

        /// <summary>
        /// Observes a property that implements INotifyPropertyChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <param name="property">The property expression. Example: ObservesProperty(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public new DelegateCommand ObservesProperty<T>(Expression<Func<T>> property)
        {
            return base.ObservesProperty(property) as DelegateCommand;
        }

        /// <summary>
        /// Observes a property that is used to determine if this command can execute, and if it implements INotifyPropertyChanged it will automatically call DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <param name="canExecute">The property expression. Example: ObservesCanExecute(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public new DelegateCommand ObservesCanExecute(Expression<Func<bool>> canExecute)
        {
            return base.ObservesCanExecute(canExecute) as DelegateCommand;
        }


    }
}
