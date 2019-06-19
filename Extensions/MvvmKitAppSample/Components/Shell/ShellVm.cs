using MvvmKit;
using MvvmKitAppSample.Components.PageOne;
using MvvmKitAppSample.Components.RegionContainer;
using MvvmKitAppSample.Model;
using MvvmKitAppSample.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKitAppSample.Components.Shell
{
    public class ShellVm : ComponentBase
    {
        #region Properties

        private string _Title;
        public string Title { get { return _Title; } set { SetProperty(ref _Title, value); } }


        private Region _MyRegion;
        public Region MyRegion { get { return _MyRegion; } set { SetProperty(ref _MyRegion, value); } }

        #endregion

        #region Commands

        #region ChangeA Command

        private DelegateCommand _ChangeACommand;
        public DelegateCommand ChangeACommand
        {
            get
            {
                if (_ChangeACommand == null) _ChangeACommand = new DelegateCommand(OnChangeACommand);
                return _ChangeACommand;
            }
        }

        public async void OnChangeACommand()
        {
            var val = await _service.PropName.Get();
            Debug.WriteLine("The value of propname is " + val);
            await _service.PropName.Set(!val);
            await _service.Method();

            await _service.Numbers.Reset(Enumerable.Range(1, 3).Select(i => i * 10));

            await _service.Numbers.Add(50);
            await _service.Numbers.Add(30);
            await _service.Numbers.Add(40);

            var items = await _service.Numbers.Items();
            Debug.WriteLine($"Items are: {string.Join(", ", items)}");

            await _service.Numbers.Remove(30);
            await _service.Numbers.RemoveAt(2);

            items = await _service.Numbers.Items();
            Debug.WriteLine($"Items are: {string.Join(", ", items)}");

            await _service.Numbers.SetAt(2, 314);

            items = await _service.Numbers.Items();
            Debug.WriteLine($"Items are: {string.Join(", ", items)}");

            await _service.Numbers.SetWhere(i => i > 45, 324);

            items = await _service.Numbers.Items();
            Debug.WriteLine($"Items are: {string.Join(", ", items)}");

            await _service.Numbers.MoveAt(1, 3);
            items = await _service.Numbers.Items();
            Debug.WriteLine($"Items are: {string.Join(", ", items)}");

            await _service.Numbers.MoveWhere(i => i > 45, 0);

            items = await _service.Numbers.Items();
            Debug.WriteLine($"Items are: {string.Join(", ", items)}");

            await _service.Numbers.Clear();
        }


        #endregion


        #region TogglePage Command

        private DelegateCommand _TogglePageCommand;
        public DelegateCommand TogglePageCommand
        {
            get
            {
                if (_TogglePageCommand == null) _TogglePageCommand = new DelegateCommand(OnTogglePageCommand);
                return _TogglePageCommand;
            }
        }

        public async void OnTogglePageCommand()
        {
            var vm = await Navigation.CurrentViewModelAt(MyRegion);
            if (vm is PageOneVm)
            {
                await Navigation.NavigateTo<RegionContainerVm>(MyRegion);
            } else
            {
                await Navigation.NavigateTo<PageOneVm>(MyRegion);
            }
        }
        #endregion



        #endregion

        private BackgroundService _service;
        private IUiService _uiService;
        private ItemsService _itemsService;

        public ShellVm()
        {
            Title = "Hello MvvmKit Runtime";
            MyRegion = new Region();
        }

        [InjectionMethod]
        public void Inject(BackgroundService service, IUiService uis, ItemsService itemsService)
        {
            _service = service;
            _uiService = uis;
            _itemsService = itemsService;
        }


        protected override async Task OnInitialized(object param)
        {
            await base.OnInitialized(param);

            await _itemsService.TodoItems.Changed.Subscribe(this, OnItemsChanged);

            var i = await _service.MyNumber.Get();
            await _service.MyNumber.Set(43);

            await _service.Numbers.Changed.Subscribe(this, changes =>
            {
                foreach (var item in changes)
                {
                    Debug.WriteLine(item);
                }
                return Tasks.Empty;
            });

            await Navigation.RegisterRegion(MyRegion);
            await Navigation.NavigateTo<PageOneVm>(MyRegion);
        }

        private Task OnItemsChanged(CollectionChanges<TodoItem> arg)
        {
            return Task.CompletedTask;
        }

        protected override async Task OnClearing()
        {
            await _service.PropName.Changed.Unsubscribe(this);
            await base.OnClearing();
        }

    }
}
