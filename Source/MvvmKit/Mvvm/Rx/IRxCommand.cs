using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmKit
{
    public interface IRxCommand<T>: ICommand, IDisposable, IObservable<T>
    {
    }

    public interface IRxCommand: IRxCommand<object>
    {
    }
}
