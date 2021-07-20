using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface INotifyDisposable: IDisposable
    {
        void Attach(IDisposable child, bool keepAlive = false);

        void AttachMany<T>(IEnumerable<T> children, bool keepAlive = false)
            where T : IDisposable;


        void Dettach(IDisposable child);

        void DettachMany<T>(IEnumerable<T> children)
            where T : IDisposable;

        bool IsDisposed { get; }
    }
}
