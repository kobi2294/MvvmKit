using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class MvvmRx
    {
        public static void LinkProperty<TVm, TProperty>(this IObservable<TProperty> source,
            TVm vm,
            Expression<Func<TVm, TProperty>> property)
            where TVm : BindableBase
        {
            var setter = property.GetProperty().ToSetter<TVm, TProperty>();
            source.Subscribe(val => setter(vm, val))
                .DisposedBy(vm);
        }

        public static void LinkCollection<TOwner, TModel, TItem, TKey>(this IObservable<ImmutableList<TModel>> source,
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


        public static TOwner LinkCollection<TOwner, TModel, TItem>(this IObservable<ImmutableList<TModel>> source,
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

        public static IObservable<TProp> PropertyValues<TBindable, TProp>(TBindable owner, Expression<Func<TBindable, TProp>> property)
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

        public static IObservable<(TProp oldValue, TProp newValue)> PropertyChanges<TBindable, TProp>(TBindable owner, Expression<Func<TBindable, TProp>> property)
            where TBindable : BindableBase
        {
            var values = PropertyValues(owner, property);
            var changes = values
                .Buffer(2, 1)
                .Where(list => list.Count == 2)
                .Select(pair => (oldValue: pair[0], newValue: pair[1]));
            return changes;
        }

        public static IObservable<(ImmutableList<T> oldValue, ImmutableList<T> newValue, NotifyCollectionChangedEventArgs args)> CollectionChanges<T>(
            ObservableCollection<T> collection)
        {
            var obs = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => collection.CollectionChanged += h,
                h => collection.CollectionChanged -= h);

            return obs.Select(x => (value: (x.Sender as ObservableCollection<T>).ToImmutableList(), args: x.EventArgs))
               .StartWith((value: collection.ToImmutableList(), args: new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)))
               .Buffer(2, 1)
               .Where(list => list.Count == 2)
               .Select(pair => (oldValue: pair[0].value, newValue: pair[1].value, args: pair[1].args));
        }
    }
}
