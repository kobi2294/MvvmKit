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
        #region Properties

        private ObservableCollection<ItemVm> _Items;
        public ObservableCollection<ItemVm> Items { get { return _Items; } set { SetProperty(ref _Items, value); } }

        #endregion

        #region Commands

        public IRxCommand AddItem { get; }

        public IRxCommand<string> RemoveItem { get; }

        #endregion

        public ViewModel()
        {
            Items = new ObservableCollection<ItemVm>().AllDisposedBy(this);
            AddItem = MvvmRx.CreateCommand(this);
            RemoveItem = MvvmRx.CreateCommand<string>(this);
        }

        public void Initialize(IObservable<ImmutableList<ItemModel>> param)
        {
            Items.CollectionChanged += Items_CollectionChanged;
            param.LinkCollection(this, Items,
                factory: () => Resolver.Resolve<ItemVm>(),
                syncer: (model, vm) => vm.ReadModel(model),
                onRemove: vm => vm.Dispose());
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine($"Collection Changed: {e.Action}, {e.NewStartingIndex}, {e.OldStartingIndex}");
        }
    }
}
