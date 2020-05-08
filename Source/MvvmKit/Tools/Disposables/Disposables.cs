using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static partial class Disposables
    {
        public static IDisposable Empty { get; } = Disposables.Call(() => { });

        public static BaseDisposableWithData<T> WithData<T>(T data)
        {
            return new BaseDisposableWithData<T>(data);
        }

        public static T DisposedBy<T>(this T child, INotifyDisposable disposer)
            where T : IDisposable
        {
            disposer.Disposing += (s, e) => child?.Dispose();
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

        public static IObservable<T> AsObservable<T>(this INotifyDisposable disposable, Func<T> lastValue = null)
        {
            return Observable.Create<T>(observer =>
            {
                if (disposable.IsDisposed)
                {
                    if (lastValue != null) observer.OnNext(lastValue());
                    observer.OnCompleted();
                    return Disposables.Empty;
                }

                EventHandler handler = (s, e) =>
                {
                    if (lastValue != null) observer.OnNext(lastValue());
                    observer.OnCompleted();
                };

                disposable.Disposing += handler;
                return Disposables.Call(() =>
                {
                    disposable.Disposing -= handler;
                });
            });
        }

        public static IObservable<T> CompletedBy<T>(this IObservable<T> source, INotifyDisposable disposable)
        {
            var completer = disposable.AsObservable<bool>(() => true);
            return source.TakeUntil(completer);
        }

    }
}
