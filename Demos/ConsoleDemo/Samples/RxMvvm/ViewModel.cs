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

        public ViewModel()
        {
            Items = new ObservableCollection<ItemVm>();
        }

        public void Initialize(IObservable<ImmutableList<ItemModel>> param)
        {
            Items.CollectionChanged += Items_CollectionChanged;
            this.Attach(param, Items, (m, vm) => vm.ReadModel(m), 
                onRemove: vm => vm.BeforeRemove());
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine($"{e.Action}, {e.NewStartingIndex}, {e.OldStartingIndex}");
        }
    }
}
