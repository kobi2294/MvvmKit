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

        public static INotifyDisposable Notifier() => new NotifyDisposableHandler();

        public static BaseDisposableWithData<T> WithData<T>(T data)
        {
            return new BaseDisposableWithData<T>(data);
        }

        public static T DisposedBy<T>(this T child, INotifyDisposable disposer)
            where T : IDisposable
        {
            if (child is Task)
            {
                throw new ArgumentException("Can not register Task for late disposal", nameof(child));
            }

            disposer.Attach(child);

            return child;
        }

        public static C AllDisposedBy<C>(this C children, INotifyDisposable disposer)
            where C : IEnumerable<IDisposable>
        {
            foreach (var item in children)
            {
                disposer.Attach(item);
            }
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

                var handler = Disposables.Call(() =>
                {
                    if (lastValue != null) observer.OnNext(lastValue());
                    observer.OnCompleted();
                });

                disposable.Attach(handler, keepAlive: true);

                return Disposables.Call(() =>
                {
                    disposable.Dettach(handler);
                });
            });
        }

        public static IObservable<T> CompletedBy<T>(this IObservable<T> source, INotifyDisposable disposable)
        {
            var completer = disposable.AsObservable<bool>(() => true);
            return source.TakeUntil(completer);
        }

        public static INotifyDisposable WhenDisposed(this INotifyDisposable source, Action action)
        {
            var call = Disposables.Call(action);
            source.Attach(call, keepAlive: true);

            return source;
        }

    }
}
