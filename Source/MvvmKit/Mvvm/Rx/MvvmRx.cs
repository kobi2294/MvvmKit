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
        public static TComponent Attach<TComponent, TProperty>(this TComponent component, 
            IObservable<TProperty> source, Expression<Func<TComponent, TProperty>> property)
            where TComponent: ComponentBase
        {
            var setter = property.GetProperty().ToSetter<TComponent, TProperty>();
            var subscription = source.Subscribe(val => setter(component, val));
            component.WhenClearing(() => subscription.Dispose());
            return component;
        }

        public static TComponent Attach<TComponent, TModel, TViewModel>(this TComponent component, 
            IObservable<ImmutableList<TModel>> source, ObservableCollection<TViewModel> target, 
            Func<TModel, TViewModel, TViewModel> syncer, Action<TViewModel> onRemove = null
            )
            where TComponent: ComponentBase           
        {
            var latestModel = ImmutableList<TModel>.Empty;
            var subscription = source.Subscribe(val =>
            {
                var diff = latestModel.Diff(val);
                latestModel = val;
                target.ApplyDiff(diff,
                    onAdd: (i, model) => syncer(model, component.Resolver.Resolve<TViewModel>()),
                    onModify: (i, oldModel, newModel, vm) => syncer(newModel, vm),
                    onRemove: (i, model, vm) => onRemove?.Invoke(vm));
            });

            component.WhenClearing(() => subscription.Dispose());
            return component;
        }
    }
}
