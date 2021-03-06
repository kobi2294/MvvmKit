﻿using MvvmKit;
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


        private string _Caption;
        public string Caption { get { return _Caption; } set { SetProperty(ref _Caption, value); } }


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
            param.ApplyOnCollection(this, Items,
                factory: () => Resolver.Resolve<ItemVm>(),
                syncer: (model, vm) => vm.ReadModel(model),
                trackBy: model => model.Uid, 
                onRemove: vm => vm.Dispose());
        }

        protected override void OnDisposed()
        {
            Console.WriteLine("Disposing ViewModel");
            base.OnDisposed();
        }
    }
}
