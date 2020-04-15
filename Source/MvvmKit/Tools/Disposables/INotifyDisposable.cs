using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface INotifyDisposable: IDisposable
    {
        event EventHandler Disposing;

        bool IsDisposed { get; }
    }
}
