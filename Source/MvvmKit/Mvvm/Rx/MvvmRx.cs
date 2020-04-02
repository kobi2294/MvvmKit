using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class MvvmRx
    {
        public static void LinkProperty<TVm, TProperty>(this IObservable<TProperty> source,
            TVm vm,
            Expression<Func<TVm, TProperty>> property)
            where TVm: BindableBase
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
            where TOwner: BindableBase
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

    }
}
