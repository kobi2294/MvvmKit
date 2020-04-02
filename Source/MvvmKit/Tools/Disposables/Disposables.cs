using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static partial class Disposables
    {
        public static BaseDisposableWithData<T> WithData<T>(T data)
        {
            return new BaseDisposableWithData<T>(data);
        }

        public static T DisposedBy<T>(this T child, INotifyDisposable disposer)
            where T : IDisposable
        {
            disposer.Disposing += (s, e) => child.Dispose();
            return child;
        }

        public static C AllDisposedBy<C>(this C children, INotifyDisposable disposer)
            where C : IEnumerable<IDisposable>
        {
            disposer.Disposing += (s, e) =>
            {
                children.ForEach(x => x.Dispose());
            };
            return children;
        }
    }
}
