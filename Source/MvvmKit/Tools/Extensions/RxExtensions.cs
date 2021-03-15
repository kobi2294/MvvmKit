using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class RxExtensions 
    {
        public static IObservable<T> WhenDifferentFrom<T>(this IObservable<T> source, IObservable<T> controller)
        {
            var res = source
                .WithLatestFrom(controller, (src, ctrl) => (src, ctrl))
                .Where(pair => !Equals(pair.src, pair.ctrl))
                .Select(pair => pair.src);

            return res;
        }

        public static IObservable<T> WhenDifferentFrom<T>(this IObservable<T> source, IObservable<T> controller, Func<T, T, bool> isEqual)
        {
            var res = source
                .WithLatestFrom(controller, (src, ctrl) => (src, ctrl))
                .Where(pair => !isEqual(pair.src, pair.ctrl))
                .Select(pair => pair.src);

            return res;
        }

        public static IObservable<E> WhenHasDifferentElementsFrom<E, T>(this IObservable<E> source, IObservable<IEnumerable<T>> controller)
            where E : IEnumerable<T>
        {
            var res = source
                .WithLatestFrom(controller, (src, ctrl) => (src, ctrl))
                .Where(pair => !pair.src.HasSameElementsAs(pair.ctrl))
                .Select(pair => pair.src);

            return res;
        }

        public static IObservable<IEnumerable<T>> DistinctUntilDifferentElements<T>(this IObservable<IEnumerable<T>> source)
        {
            return source
                .DistinctUntilChanged(ComparerExtensions.SameElementsComparer<T>());
        }

        public static IObservable<T> DistinctUntilChanged<T>(this IObservable<T> source, Func<T, T, bool> isEqual)
        {
            var comparer = isEqual.ToEqualityComparer();
            return source.DistinctUntilChanged(comparer);
        }

        public static IObservable<T> PublishAndConnect<T>(this IObservable<T> source, INotifyDisposable owner, T initialValue)
        {
            var res = source
                .Publish(initialValue);

            res
                .Connect()
                .DisposedBy(owner);

            return res;
        }

        public static IObservable<T> PublishAndConnect<T>(this IObservable<T> source, INotifyDisposable owner)
        {
            var res = source.Publish();
            res.Connect()
                .DisposedBy(owner);

            return res;
        }


    }

}
