﻿using MvvmKit;
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

        #region TestThreads Command

        private DelegateCommand _TestThreadsCommand;
        public DelegateCommand TestThreadsCommand
        {
            get
            {
                if (_TestThreadsCommand == null) _TestThreadsCommand = new DelegateCommand(OnTestThreadsCommand);
                return _TestThreadsCommand;
            }
        }

        public async void OnTestThreadsCommand()
        {
            await _service.Method3();
        }
        #endregion

        #region ChangeState Command

        private DelegateCommand _ChangeStateCommand;
        public DelegateCommand ChangeStateCommand
        {
            get
            {
                if (_ChangeStateCommand == null) _ChangeStateCommand = new DelegateCommand(OnChangeStateCommand);
                return _ChangeStateCommand;
            }
        }

        public async void OnChangeStateCommand()
        {
            await _state.Method();
        }
        #endregion

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

            await _service.Numbers.Modify(list =>
            {
                list.Reset(Enumerable.Range(1, 3).Select(i => i * 10));
                list.Add(50);
                list.Add(30);
                list.Add(40);
            });

            var items = await _service.Numbers.Get();
            Debug.WriteLine($"Items are: {string.Join(", ", items)}");

            await _service.Numbers.Modify(list =>
            {
                list.Remove(30);
                list.RemoveAt(2);
            });

            items = await _service.Numbers.Get();
            Debug.WriteLine($"Items are: {string.Join(", ", items)}");

            await _service.Numbers.Modify(list => list[2] = 314);

            items = await _service.Numbers.Get();
            Debug.WriteLine($"Items are: {string.Join(", ", items)}");

            await _service.Numbers.Modify(list => list.SetWhere(i => i > 45, 324));

            items = await _service.Numbers.Get();
            Debug.WriteLine($"Items are: {string.Join(", ", items)}");

            await _service.Numbers.Modify(list => list.MoveAt(1, 3));
            items = await _service.Numbers.Get();
            Debug.WriteLine($"Items are: {string.Join(", ", items)}");

            await _service.Numbers.Modify(list =>
            {
                var item = list.FirstOrDefault(i => i > 45);
                if (item > 0)
                    list.MoveItem(item, 0);
            });

            items = await _service.Numbers.Get();
            Debug.WriteLine($"Items are: {string.Join(", ", items)}");

            await _service.Numbers.Modify(list => list.Clear());
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
            var vm = await (await Navigation.CurrentViewModelAt(MyRegion)).Get();
            if (vm is PageOneVm)
            {
                await Navigation.NavigateTo<RegionContainerVm>(MyRegion);
            } else
            {
                await Navigation.NavigateTo<PageOneVm>(MyRegion);
            }
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
            await _dialogs.Message("My region is: " + RegionService.Region.Name);
        }
        #endregion

        #endregion

        private BackgroundService _service;
        private IUiService _uiService;
        private ItemsService _itemsService;
        private DialogsService _dialogs;
        private StateService _state;

        public ShellVm()
        {
            Title = "Hello MvvmKit Runtime";
            MyRegion = new Region().WithName("Inside Shell");
        }

        [InjectionMethod]
        public void Inject(
            BackgroundService service, 
            IUiService uis, 
            ItemsService itemsService, 
            DialogsService dialogs, 
            StateService state)
        {
            _service = service;
            _uiService = uis;
            _itemsService = itemsService;
            _dialogs = dialogs;
            _state = state;
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

            await _state.IsMuted.Changed.Subscribe(this, b =>
            {
                Debug.WriteLine("State has changed, IsMuted = " + b);
                return Task.CompletedTask;
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
