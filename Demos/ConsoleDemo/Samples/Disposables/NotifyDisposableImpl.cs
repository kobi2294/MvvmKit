using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Disposables
{
    public class NotifyDisposableImpl : BaseDisposable
    {
        protected override void OnDisposed()
        {
            base.OnDisposed();
            Console.WriteLine("Disposing Notifty Disposable Impl");
        }
    }
}
