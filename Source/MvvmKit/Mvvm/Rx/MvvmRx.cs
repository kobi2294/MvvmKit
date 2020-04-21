using MvvmKit.Mvvm.Rx.StoreHistory;
using ReduxSimple;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class MvvmRx
    {
        #region Observable<CollectionChanges> => Observable<Diff>

        /// <summary>
        /// Converts stream of collection changes to stream of DiffResult. Note that each IChange is converted to 
        /// A single DiffResult so the stream of DiffResult is a flattened collection of collection of changes. We use
        /// Scheduler.Immediate to make sure that a set of DiffResult is completed before the next one begins, even if 
        /// 2 CollectionChanges arrive at the same time
        /// </summary>
        public static IObservable<DiffResults<T>> ToDiffResult<T>(this IObservable<CollectionChanges<T>> source)
        {
            return source.SelectMany(changes =>
                changes.Select(change => change.ToDiffResult())
                .ToObservable(Scheduler.Immediate));
        }

        /// <summary>
        /// Converts a single CollectionChanges struct to an observable of DiffResult records. Note that each 
        /// CollectionChanges instance is converted into a collection of DiffResult instances, and they are fired
        /// using the Immediate scheduler, to make sure they all run at the original order even when concatinated to 
        /// other DiffResults
        /// </summary>
        public static IObservable<DiffResults<T>> ToDiffResult<T>(this CollectionChanges<T> source)
        {
            return source
                .Select(change => change.ToDiffResult())
                .ToObservable(Scheduler.Immediate);
        }

        #endregion

        #region IStateReader => Observable

        /// <summary>
        /// Subscribes to an IStatePropertyReader and exposes it as Observable
        /// </summary>
        public static IObservable<T> ObserveValue<T, TOwner>(this IStatePropertyReader<T> source, TOwner owner)
            where TOwner : INotifyDisposable
        {
            return Observable.Create<T>(async observer =>
            {
                Func<T, Task> handler = val =>
                {
                    observer.OnNext(val);
                    return Task.CompletedTask;
                };

                await source.Changed.Subscribe(owner, handler);
                return Disposables.Call(() =>
                {
                    source.Changed.Unsubscribe(owner, handler);
                });
            });
        }

        /// <summary>
        /// Subscribes to an IStateCollectionReader and exposes it as Observable of CollectionChanges
        /// </summary>
        public static IObservable<CollectionChanges<T>> ObserveChanges<T, TOwner>(this IStateCollectionReader<T> source, TOwner owner)
            where TOwner : INotifyDisposable
        {
            return Observable.Create<CollectionChanges<T>>(async observer =>
            {
                Func<CollectionChanges<T>, Task> handler = val =>
                {
                    observer.OnNext(val);
                    return Task.CompletedTask;
                };

                await source.Changed.Subscribe(owner, handler);
                return Disposables.Call(() =>
                {
                    source.Changed.Unsubscribe(owner, handler);
                });
            });
        }

        /// <summary>
        /// Subscribes to a StateCollectionReader and exposes it as collection of (immuable) list values
        /// </summary>
        public static IObservable<ImmutableList<T>> ObserveValues<T, TOwner>(this IStateCollectionReader<T> source, TOwner owner)
            where TOwner : INotifyDisposable
        {
            return source.ObserveChanges(owner).Select(changeSet => changeSet.NewValues.ToImmutableList());
        }

        /// <summary>
        /// Subscribes to a StateCollectionReader and exposes it as an observable of DiffResult. Note that each IChange
        /// is converted into a single DiffResult, so one StateCollectionReader Change event may yield many DiffResult instances.
        /// You can use the result of this method to apply changes onto an observable collection
        /// </summary>
        public static IObservable<DiffResults<T>> ObserveDiffs<T, TOwner>(this IStateCollectionReader<T> source, TOwner owner)
            where TOwner : INotifyDisposable
        {
            return source.ObserveChanges(owner).ToDiffResult();
        }

        #endregion

        #region Observable<Collection> => Observable<Diff>

        /// <summary>
        /// Use this method to convert an observable of collection values, to observable of diffs. The result observable
        /// Performs Diff algorithm between each 2 consecutive collections and yields DiffResult instance with all their differences
        /// This DiffResult may be applied onto an observable collection
        /// </summary>
        public static IObservable<DiffResults<T>> ObserveDiff<T>(this IObservable<ImmutableList<T>> source)
        {
            var res = source.Scan((state: ImmutableList<T>.Empty, diff: DiffResults<T>.Empty),
                (acc, newList) => (state: newList, diff: acc.state.Diff(newList)))
                .Select(pair => pair.diff)
                .Skip(1);
            return res;
        }

        /// <summary>
        /// Use this method to convert an observable of collection values, to observable of diffs. The result observable
        /// Performs Diff algorithm between each 2 consecutive collections and yields DiffResult instance with all their differences
        /// This DiffResult may be applied onto an observable collection
        /// </summary>
        public static IObservable<DiffResults<T>> ObserveDiff<T, TKey>(this IObservable<ImmutableList<T>> source,
            Func<T, TKey> trackBy)
        {
            var res = source.Scan((state: ImmutableList<T>.Empty, diff: DiffResults<T>.Empty),
                (acc, newList) => (state: newList, diff: acc.state.Diff(newList, trackBy)))
                .Select(pair => pair.diff)
                .Skip(1);
            return res;
        }

        #endregion

        #region Observable => Apply on View Model members

        /// <summary>
        /// Applies an Observable of values onto a view model property. 
        /// </summary>
        public static void ApplyOnProperty<TVm, TProperty>(this IObservable<TProperty> source,
            TVm vm,
            Expression<Func<TVm, TProperty>> property,
            Action<TProperty> onOldValue = null)
            where TVm : BindableBase
        {
            var setter = property.GetProperty().ToSetter<TVm, TProperty>();
            Action beforeChange = null;
            if (onOldValue != null)
            {
                var getter = property.GetProperty().ToGetter<TVm, TProperty>();
                beforeChange = () => onOldValue(getter(vm));
            }

            source
                .ObserveOnDispatcher()
                .Subscribe(val =>
            {
                beforeChange?.Invoke();
                setter(vm, val);
            })
                .DisposedBy(vm);
        }

        /// <summary>
        /// Applies Observable of DiffResults onto view model ObservableCollection
        /// </summary>
        public static void ApplyOnCollection<TOwner, TModel, TItem>(this IObservable<DiffResults<TModel>> diffs,
            TOwner owner,
            ObservableCollection<TItem> targetCollection,
            Func<TItem> factory,
            Func<TModel, TItem, TItem> syncer,
            Action<TItem> onRemove = null)
                    where TOwner : BindableBase
        {
            diffs
                .ObserveOnDispatcher()
                .Subscribe(diff =>
            {
                targetCollection.ApplyDiff(diff,
                    onAdd: (index, model) => syncer(model, factory()),
                    onModify: (index, oldModel, newModel, item) => syncer(newModel, item),
                    onRemove: (index, model, vm) => onRemove?.Invoke(vm)
                    );
            }).DisposedBy(owner);
        }


        /// <summary>
        /// Applies observable of list values onto view model observable collection. Note that this method will
        /// Perform diff between each consecutive lists, in order to calculate the smallest set of changes needed to be performed 
        /// the observable collection (using Diff algorithm and DiffResult instances)
        /// </summary>
        /// <param name="factory">A method to be used as item instance factory</param>
        /// <param name="syncer">A method to be used to apply changes in model on item</param>
        /// <param name="trackBy">A method to be used as key selector, so the diff algorithm can recognize changed items</param>
        /// <param name="onRemove">A method to be used before removing an item of the target collection</param>
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
            source
                .ObserveOnDispatcher()
                .Subscribe(val =>
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


        /// <summary>
        /// Applies observable of list values onto view model observable collection. Note that this method will
        /// Perform diff between each consecutive lists, in order to calculate the smallest set of changes needed to be performed 
        /// the observable collection (using Diff algorithm and DiffResult instances)
        /// </summary>
        /// <param name="factory">A method to be used as item instance factory</param>
        /// <param name="syncer">A method to be used to apply changes in model on item</param>
        /// <param name="onRemove">A method to be used before removing an item of the target collection</param>
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
            source
                .ObserveOnDispatcher()
                .Subscribe(val =>
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

        #endregion

        #region Create Rx Command

        /// <summary>
        /// Creates a RxCommand, which is exposed as an observable, and may consume a CanExecute observable
        /// </summary>
        public static IRxCommand CreateCommand(this BindableBase owner)
        {
            return new RxCommand()
                .DisposedBy(owner);
        }

        /// <summary>
        /// Creates a RxCommand of T, which is exposed as an observable, and may consume a CanExecute observable
        /// </summary>
        public static IRxCommand<T> CreateCommand<T>(this BindableBase owner)
        {
            return new RxCommand<T>()
                .DisposedBy(owner);                
        }

        #endregion

        #region View Model members (property, collection) => Observable

        /// <summary>
        /// Creates an observable that yields an event when the property value changes. The event payload is the new
        /// property value.
        /// </summary>
        public static IObservable<TProp> ObservePropertyValues<TBindable, TProp>(this TBindable owner, Expression<Func<TBindable, TProp>> property)
            where TBindable : BindableBase
        {
            var propInfo = property.GetProperty();
            var getter = propInfo.ToGetter<TBindable, TProp>();
            var value = getter(owner);
            return Observable.Create<TProp>(observer =>
            {
                owner.Observe<TProp>(propInfo.Name, val => observer.OnNext(val));

                EventHandler handler = (s, e) => observer.OnCompleted();
                owner.Disposing += handler;

                return Disposables.Call(() =>
                {
                    owner.Unobserve(propInfo.Name, owner);
                    owner.Disposing -= handler;
                });
            }).ObserveOnDispatcher();
        }

        /// <summary>
        /// Creates an observable that yields an event when the property value changes. The event payload is a
        /// tupple with the old value and the new value
        /// </summary>
        public static IObservable<(TProp oldValue, TProp newValue)> ObservePropertyChanges<TBindable, TProp>(this TBindable owner, Expression<Func<TBindable, TProp>> property)
            where TBindable : BindableBase
        {
            var values = ObservePropertyValues(owner, property);
            var changes = values
                .Buffer(2, 1)
                .Where(list => list.Count == 2)
                .Select(pair => (oldValue: pair[0], newValue: pair[1]));
            return changes;
        }


        /// <summary>
        /// Creates an observerable that yields an event when the observable collection changes. The event payload is
        /// a tupple, holding the new values (as an immutable list) and the collection changed event args
        /// </summary>
        public static IObservable<(ImmutableList<T> values, NotifyCollectionChangedEventArgs args)>
            ObserveCollectionChanges<TBindable, T>(TBindable owner, ObservableCollection<T> collection)
            where TBindable : INotifyDisposable
        {
            var obs = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => collection.CollectionChanged += h,
                h => collection.CollectionChanged -= h);

            return obs.Select(x => (values: (x.Sender as ObservableCollection<T>).ToImmutableList(), args: x.EventArgs))
               .StartWith((values: collection.ToImmutableList(), args: new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)))
               .CompletedBy(owner)
               .ObserveOnDispatcher();
        }

        /// <summary>
        /// Creates an observerable that yields an event when the observable collection changes. The event payload is
        /// a tupple, holding the new values (as an immutable list), the old values (also as an immutable list)
        /// and the collection changed event args
        /// </summary>
        public static IObservable<(ImmutableList<T> oldValue, ImmutableList<T> newValue, NotifyCollectionChangedEventArgs args)> 
            ObserveCollectionChangesExtended<TBindable, T>(TBindable owner, ObservableCollection<T> collection)
            where TBindable: INotifyDisposable
        {
            return ObserveCollectionChanges(owner, collection)
               .Buffer(2, 1)
               .Where(list => list.Count == 2)
               .Select(pair => (oldValue: pair[0].values, newValue: pair[1].values, args: pair[1].args));
        }

        /// <summary>
        /// Creates an observerable that yields an event when the observable collection changes. The event payload is
        /// a an immutable list of the collection values
        /// </summary>
        public static IObservable<ImmutableList<T>> 
            ObserveCollectionValues<TBindable, T>(TBindable owner, ObservableCollection<T> collection)
            where TBindable: INotifyDisposable
        {
            return ObserveCollectionChanges(owner, collection)
                .Select(pair => pair.values);
        }


        #region Privates
        /// <summary>
        /// For each item in the items list, extracts the selected observable and subscribes to it, while 
        /// adding the subscription to the dictionary
        /// </summary>
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

        /// <summary>
        /// Loops through the items as keys. Disposes their IDisposable values, and removes them from the dictionary
        /// </summary>
        private static void _unsubscribeFromItems<TItem>(IEnumerable<TItem> items, Dictionary<TItem, IDisposable> subscriptions)
        {
            foreach (var item in items)
            {
                var disposable = subscriptions[item];
                disposable.Dispose();
                subscriptions.Remove(item);
            }
        }

        /// <summary>
        /// Loops through all the items in the dictionary, and disposes them. Finally, clears the dictionary
        /// </summary>
        private static void _clearSubscriptions<TItem>(Dictionary<TItem, IDisposable> subscriptions)
        {
            foreach (var pair in subscriptions)
            {
                var disposable = pair.Value;
                disposable.Dispose();
            }
            subscriptions.Clear();
        }

        /// <summary>
        /// Subscribes to the observable collection changes, and responds to each action type by subscribing and unsubscribing
        /// to their selected observables. Returns a disposable to triggers unsubscription and disposal of all 
        /// sub-subscriptions
        /// </summary>
        private static IDisposable _subscribeCollectMany<TBindable, TItem, TObservable, T>(
            TBindable owner, 
            ObservableCollection<TItem> collection,
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
        #endregion

        /// <summary>
        /// For each item in an observable collection, select an observable, and merge all their notifications into 
        /// a single observable
        /// </summary>
        public static IObservable<T> ObserveMany<TBindable, TItem, TObservable, T>(TBindable owner, ObservableCollection<TItem> collection, 
            Func<TItem, IObservable<TObservable>> selector, 
            Func<TItem, TObservable, T> value)
            where TBindable: INotifyDisposable
        {
            return Observable
                .Create((IObserver<T> observer) => _subscribeCollectMany(owner, collection, selector, value, observer))
                .CompletedBy(owner);
        }

        #endregion

        #region Helpers and Utilities

        /// <summary>
        /// Subscribes an asynchronous observer to an observable
        /// </summary>
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

        /// <summary>
        /// Opens the Redux Store History browser window
        /// </summary>
        public static async Task<IDisposable> OpenHistoryBrowser<T>(this ReduxStore<T> store, NavigationService navigation)
            where T : class, IImmutable, new()
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

        public static IObservable<T> WhenDifferentFrom<T>(this IObservable<T> source, IObservable<T> controller)
        {
            var res = source
                .WithLatestFrom(controller, (src, ctrl) => (src, ctrl))
                .Where(pair => !Equals(pair.src, pair.ctrl))
                .Select(pair => pair.src);

            return res;
        }

        #endregion

    }
}
