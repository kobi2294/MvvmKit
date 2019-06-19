using MvvmKit;
using MvvmKitAppSample.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKitAppSample.Components.PageOne
{
    public class PageOneVm : ComponentBase
    {
        #region Properties

        private ObservableCollection<ItemVm> _Items;
        public ObservableCollection<ItemVm> Items { get { return _Items; } set { SetProperty(ref _Items, value); } }


        private string _SelectedItem;
        public string SelectedItem { get { return _SelectedItem; } set { SetProperty(ref _SelectedItem, value); } }


        #endregion

        #region Commands

        #region Select Command

        private DelegateCommand<ItemVm> _SelectCommand;
        public DelegateCommand<ItemVm> SelectCommand
        {
            get
            {
                if (_SelectCommand == null) _SelectCommand = new DelegateCommand<ItemVm>(OnSelectCommand);
                return _SelectCommand;
            }
        }

        public void OnSelectCommand(ItemVm param)
        {
            SelectedItem = param.Value;
        }

        #endregion

        #region Add Command

        private DelegateCommand<string> _AddCommand;
        public DelegateCommand<string> AddCommand
        {
            get
            {
                if (_AddCommand == null) _AddCommand = new DelegateCommand<string>(OnAddCommand);
                return _AddCommand;
            }
        }

        public async void OnAddCommand(string param)
        {
            await _service.MyNames.Add(param);
        }

        #endregion


        #region MyRegion Command

        private DelegateCommand _MyRegionCommand;
        public DelegateCommand MyRegionCommand
        {
            get
            {
                if (_MyRegionCommand == null) _MyRegionCommand = new DelegateCommand(OnMyRegionCommand);
                return _MyRegionCommand;
            }
        }

        public async void OnMyRegionCommand()
        {
            await _dialogs.Message("My region is " + Region.Name);
        }
        #endregion



        #endregion

        private BackgroundService _service;
        private DialogsService _dialogs;
        private ServiceCollectionAdapter<string, ItemVm> _adapter;

        public PageOneVm()
        {
            Items = new ObservableCollection<ItemVm>();
        }

        [InjectionMethod]
        public void Inject(BackgroundService service, DialogsService dialogs)
        {
            _service = service;
            _dialogs = dialogs;
        }

        public async Task OnChildRemove(ItemVm vm)
        {
            await _service.MyNames.Remove(vm.Value);
        }

        protected override async Task OnInitialized(object param)
        {
            await base.OnInitialized(param);
            _adapter = await Resolver.Resolve<ServiceCollectionAdapter<string, ItemVm>>()
                .From(_service.MyNames)
                .To(Items)
                .ModifyWith(async (m, vm) => await vm.ReadModel(m, this))
                .Start();            
        }

        protected override async Task OnClearing()
        {
            await _adapter.Stop();
            await base.OnClearing();
        }

    }
}
