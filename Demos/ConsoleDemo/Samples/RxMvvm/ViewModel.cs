using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.RxMvvm
{
    public class ViewModel: ComponentBase
    {
        private ImmutableList<ItemModel> _latestModel = ImmutableList<ItemModel>.Empty;
        private IDisposable _subscription = null;

        #region Properties

        private ObservableCollection<ItemVm> _Items;
        public ObservableCollection<ItemVm> Items { get { return _Items; } set { SetProperty(ref _Items, value); } }

        #endregion

        public ViewModel()
        {
            Items = new ObservableCollection<ItemVm>();
        }

        public void Attach(IObservable<ImmutableList<ItemModel>> source, ObservableCollection<ItemVm> target, Func<ItemModel, ItemVm, ItemVm> sync)
        {
            _subscription = source.Subscribe(val =>
            {
                var diff = _latestModel.Diff(val);
                _latestModel = val;
                Items.ApplyDiff(diff,
                    onAdd: (i, model) => sync(model, Resolver.Resolve<ItemVm>()),
                    onModify: (i, oldModel, newModel, vm) => sync(newModel, vm), 
                    onRemove: (i, model, vm) => vm.BeforeRemove()
                    );
            });
        }

        public void Initialize(IObservable<ImmutableList<ItemModel>> param)
        {
            Items.CollectionChanged += Items_CollectionChanged;
            Attach(param, Items, (model, vm) => vm.ReadModel(model));

        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine($"{e.Action}, {e.NewStartingIndex}, {e.OldStartingIndex}");
        }
    }
}
