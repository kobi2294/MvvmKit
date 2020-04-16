using MvvmKit.Mvvm.Rx.StoreHistory;
using ReduxSimple;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class MvvmRx
    {
        public static void ApplyOnProperty<TVm, TProperty>(this IObservable<TProperty> source,
            TVm vm,
            Expression<Func<TVm, TProperty>> property)
            where TVm : BindableBase
        {
            var setter = property.GetProperty().ToSetter<TVm, TProperty>();
            source.Subscribe(val => setter(vm, val))
                .DisposedBy(vm);
        }

        public static void ApplyOnCollection<TOwner, TModel, TItem, TKey>(this IObservable<ImmutableList<TModel>> source,
            TOwner owner,
            ObservableCollection<TItem> targetCollection,
            Func<TItem> factory,
            Func<TModel, TItem, TItem> syncer,
            Func<TModel, TKey> trackBy,
            Action<TItem> onRemove = null
            )
        where TOwner : BindableBase
        {
            var latestModel = ImmutableList<TModel>.Empty;
            source.Subscribe(val =>
            {
                var diff = latestModel.Diff(val, trackBy);
                latestModel = val;
                targetCollection.ApplyDiff(diff,
                    onAdd: (index, model) => syncer(model, factory()),
                    onModify: (index, oldModel, newModel, item) => syncer(newModel, item),
                    onRemove: (index, model, vm) => onRemove?.Invoke(vm)
                    );
            }).DisposedBy(owner);
        }


        public static TOwner ApplyOnCollection<TOwner, TModel, TItem>(this IObservable<ImmutableList<TModel>> source,
            TOwner owner,
            ObservableCollection<TItem> targetCollection,
            Func<TItem> factory,
            Func<TModel, TItem, TItem> syncer,
            Action<TItem> onRemove = null
            )
            where TOwner : BindableBase
        {
            var latestModel = ImmutableList<TModel>.Empty;
            source.Subscribe(val =>
            {
                var diff = latestModel.Diff(val);
                latestModel = val;
                targetCollection.ApplyDiff(diff,
                    onAdd: (i, model) => syncer(model, factory()),
                    onModify: (i, oldModel, newModel, vm) => syncer(newModel, vm),
                    onRemove: (i, model, vm) => onRemove?.Invoke(vm));
            }).DisposedBy(owner);

            return owner;
        }

        public static IRxCommand CreateCommand(BindableBase owner)
        {
            return new RxCommand()
                .DisposedBy(owner);
        }

        public static IRxCommand CreateCommand(BindableBase owner, IObservable<bool> canExecute)
        {
            return new RxCommand<bool>(canExecute, t => t)
                .DisposedBy(owner);
        }

        public static IRxCommand CreateCommand<TCanExecute>(BindableBase owner,
            IObservable<TCanExecute> canExecuteObservable, Func<TCanExecute, bool> canExecuteFunc)
        {
            return new RxCommand<TCanExecute>(canExecuteObservable, canExecuteFunc)
                .DisposedBy(owner);
        }

        public static IRxCommand<T> CreateCommand<T>(BindableBase owner)
        {
            return new RxCommand<T, bool>()
                .DisposedBy(owner);
        }

        public static IRxCommand<T> CreateCommand<T>(BindableBase owner, IObservable<bool> canExecute)
        {
            return new RxCommand<T, bool>(canExecute, (t, cx) => cx)
                .DisposedBy(owner);
        }

        public static IRxCommand<T> CreateCommand<T, TCanExecute>(BindableBase owner,
            IObservable<TCanExecute> canExecuteObservable, Func<T, TCanExecute, bool> canExecuteFunc)
        {
            return new RxCommand<T, TCanExecute>(canExecuteObservable, (t, cx) => canExecuteFunc(t, cx))
                .DisposedBy(owner);
        }

        public static IObservable<TProp> ObservePropertyValues<TBindable, TProp>(TBindable owner, Expression<Func<TBindable, TProp>> property)
            where TBindable : BindableBase
        {
            var propInfo = property.GetProperty();
            var getter = propInfo.ToGetter<TBindable, TProp>();
            var value = getter(owner);
            var subject = new BehaviorSubject<TProp>(value);
            owner.Observe<TProp>(propInfo.Name, val => subject.OnNext(val));
            owner.Disposing += (s, e) => subject.OnCompleted();
            return subject.AsObservable();
        }

        public static IObservable<(TProp oldValue, TProp newValue)> ObservePropertyChanges<TBindable, TProp>(TBindable owner, Expression<Func<TBindable, TProp>> property)
            where TBindable : BindableBase
        {
            var values = ObservePropertyValues(owner, property);
            var changes = values
                .Buffer(2, 1)
                .Where(list => list.Count == 2)
                .Select(pair => (oldValue: pair[0], newValue: pair[1]));
            return changes;
        }


        public static IObservable<(ImmutableList<T> values, NotifyCollectionChangedEventArgs args)>
            ObserveCollectionChanges<TBindable, T>(TBindable owner, ObservableCollection<T> collection)
            where TBindable : INotifyDisposable
        {
            var obs = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => collection.CollectionChanged += h,
                h => collection.CollectionChanged -= h);

            return obs.Select(x => (values: (x.Sender as ObservableCollection<T>).ToImmutableList(), args: x.EventArgs))
               .StartWith((values: collection.ToImmutableList(), args: new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)))
               .CompletedBy(owner);
        }

        public static IObservable<(ImmutableList<T> oldValue, ImmutableList<T> newValue, NotifyCollectionChangedEventArgs args)> 
            ObserveCollectionChangesExtended<TBindable, T>(TBindable owner, ObservableCollection<T> collection)
            where TBindable: INotifyDisposable
        {
            return ObserveCollectionChanges(owner, collection)
               .Buffer(2, 1)
               .Where(list => list.Count == 2)
               .Select(pair => (oldValue: pair[0].values, newValue: pair[1].values, args: pair[1].args));
        }

        public static IObservable<ImmutableList<T>> 
            ObserveCollectionValues<TBindable, T>(TBindable owner, ObservableCollection<T> collection)
            where TBindable: INotifyDisposable
        {
            return ObserveCollectionChanges(owner, collection)
                .Select(pair => pair.values);
        }

        private static void _subscribeToNewItems<TItem, TObservable, T>(IEnumerable<TItem> items, 
            Func<TItem, IObservable<TObservable>> selector, 
            Func<TItem, TObservable, T> value, 
            Dictionary<TItem, IDisposable> subscriptions, 
            IObserver<T> observer)
        {
            foreach (var item in items)
            {
                var observable = selector(item);
                var subscription = observable.Subscribe(Observer.Create<TObservable>(val =>
                {
                    var finalValue = value(item, val);
                    observer.OnNext(finalValue);
                }, observer.OnError, observer.OnCompleted));
                subscriptions.Add(item, subscription);
            }
        }

        private static void _unsubscribeFromItems<TItem>(IEnumerable<TItem> items, Dictionary<TItem, IDisposable> subscriptions)
        {
            foreach (var item in items)
            {
                var disposable = subscriptions[item];
                disposable.Dispose();
                subscriptions.Remove(item);
            }
        }

        private static void _clearSubscriptions<TItem>(Dictionary<TItem, IDisposable> subscriptions)
        {
            foreach (var pair in subscriptions)
            {
                var disposable = pair.Value;
                disposable.Dispose();
            }
            subscriptions.Clear();
        }

        private static IDisposable _subscribeCollectMany<TBindable, TItem, TObservable, T>(TBindable owner, ObservableCollection<TItem> collection,
            Func<TItem, IObservable<TObservable>> selector,
            Func<TItem, TObservable, T> value, 
            IObserver<T> observer)
            where TBindable : INotifyDisposable
        {
            var subscriptions = new Dictionary<TItem, IDisposable>();
            var mainSubscription = ObserveCollectionChanges(owner, collection)
                    .Synchronize()
                    .Subscribe(pair =>
                    {
                        switch (pair.args.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                                _subscribeToNewItems(pair.args.NewItems.Cast<TItem>(), selector, value, subscriptions, observer);
                                break;
                            case NotifyCollectionChangedAction.Remove:
                                _unsubscribeFromItems(pair.args.OldItems.Cast<TItem>(), subscriptions);
                                break;
                            case NotifyCollectionChangedAction.Replace:
                                _unsubscribeFromItems(pair.args.OldItems.Cast<TItem>(), subscriptions);
                                _subscribeToNewItems(pair.args.NewItems.Cast<TItem>(), selector, value, subscriptions, observer);
                                break;
                            case NotifyCollectionChangedAction.Move:
                                break;
                            case NotifyCollectionChangedAction.Reset:
                                _clearSubscriptions(subscriptions);
                                _subscribeToNewItems(pair.values, selector, value, subscriptions, observer);
                                break;
                            default:
                                break;
                        }
                    },
                    err =>
                    {
                        _clearSubscriptions(subscriptions);
                    },
                    () =>
                    {
                        _clearSubscriptions(subscriptions);
                    });

            return Disposable.Create(() =>
            {
                _clearSubscriptions(subscriptions);
                mainSubscription.Dispose();
            });


        }

        public static IObservable<T> ObserveMany<TBindable, TItem, TObservable, T>(TBindable owner, ObservableCollection<TItem> collection, 
            Func<TItem, IObservable<TObservable>> selector, 
            Func<TItem, TObservable, T> value)
            where TBindable: INotifyDisposable
        {
            return Observable
                .Create((IObserver<T> observer) => _subscribeCollectMany(owner, collection, selector, value, observer))
                .CompletedBy(owner);
        }

        public static IDisposable SubscribeAsync<T>(this IObservable<T> source, Func<T, Task> action)
        {
            return source
                .SelectMany(async t =>
                {
                    await action(t);
                    return Unit.Default;
                })
                .Subscribe();                
        }

        public static async Task<IDisposable> OpenHistoryBrowser<T>(this ReduxStore<T> store, NavigationService navigation)
            where T: class, IImmutable, new()
        {
            var region = new Region()
                .WithName("Store Browser")
                .Add(new OpenWindowRegionBehavior());

            await navigation.RegisterRegion(region);
            var vm = await navigation.NavigateTo<StoreHistoryVm>(region);
            vm.ConnectToStore(store);

            return Disposables.Call(async () =>
            {
                await navigation.Clear(region);
                await navigation.UnregisterRegion(region);
            });
        }
    }
}
