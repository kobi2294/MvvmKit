using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    namespace MvvmKit
    {
        public class DelegateCommand<T> : Prism.Commands.PrismDelegateCommand<T>
        {
            /// <summary>
            /// Initializes a new instance of <see cref="DelegateCommand{T}"/>.
            /// </summary>
            /// <param name="execute">Delegate to execute when Execute is called on the command. This can be null to just hook up a CanExecute delegate.</param>
            /// <remarks><see cref="CanExecute"/> will always return true.</remarks>
            public DelegateCommand(Action<T> execute)
                : base(execute)
            {
            }

            /// <summary>
            /// Initializes a new instance of <see cref="DelegateCommand{T}"/>.
            /// </summary>
            /// <param name="execute">Delegate to execute when Execute is called on the command. This can be null to just hook up a CanExecute delegate.</param>
            /// <param name="canExecute">Delegate to execute when CanExecute is called on the command. This can be null.</param>
            /// <exception cref="ArgumentNullException">When both <paramref name="execute"/> and <paramref name="canExecute"/> ar <see langword="null" />.</exception>
            DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
                : base(execute, canExecute)
            {
            }

            /// <summary>
            /// Observes a property that implements INotifyPropertyChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
            /// </summary>
            /// <typeparam name="TType">The type of the return value of the method that this delegate encapulates</typeparam>
            /// <param name="property">The property expression. Example: ObservesProperty(() => PropertyName).</param>
            /// <returns>The current instance of DelegateCommand</returns>
            public new DelegateCommand<T> ObservesProperty<TType>(Expression<Func<TType>> property)
            {
                return base.ObservesProperty(property) as DelegateCommand<T>;
            }

            /// <summary>
            /// Observes a property that is used to determine if this command can execute, and if it implements INotifyPropertyChanged it will automatically call DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
            /// </summary>
            /// <param name="canExecuteExpression">The property expression. Example: ObservesCanExecute(() => PropertyName).</param>
            /// <returns>The current instance of DelegateCommand</returns>
            public new DelegateCommand<T> ObservesCanExecute(Expression<Func<bool>> canExecute)
            {
                return base.ObservesCanExecute(canExecute) as DelegateCommand<T>;
            }

        }
    }
}
