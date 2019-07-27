using MvvmKit;
using MvvmKitAppSample.Components.PageOne;
using MvvmKitAppSample.Components.PageTwo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKitAppSample.Components.RegionContainer
{
    public class RegionContainerVm : ComponentBase
    {
        #region Properties

        private Region _MyRegion;
        public Region MyRegion { get { return _MyRegion; } set { SetProperty(ref _MyRegion, value); } }

        private bool _CanBack;
        public bool CanBack { get { return _CanBack; } set { SetProperty(ref _CanBack, value); } }

        #endregion

        #region Commands

        #region PageOne Command

        private DelegateCommand _PageOneCommand;
        public DelegateCommand PageOneCommand
        {
            get
            {
                if (_PageOneCommand == null) _PageOneCommand = new DelegateCommand(OnPageOneCommand);
                return _PageOneCommand;
            }
        }

        public async void OnPageOneCommand()
        {
            await Navigation.NavigateTo<PageOneVm>(MyRegion);
        }
        #endregion

        #region PageTwo Command

        private DelegateCommand _PageTwoCommand;
        public DelegateCommand PageTwoCommand
        {
            get
            {
                if (_PageTwoCommand == null) _PageTwoCommand = new DelegateCommand(OnPageTwoCommand);
                return _PageTwoCommand;
            }
        }

        public async void OnPageTwoCommand()
        {
            await Navigation.NavigateTo<PageTwoVm>(MyRegion);
        }
        #endregion

        #region Back Command

        private DelegateCommand _BackCommand;
        public DelegateCommand BackCommand
        {
            get
            {
                if (_BackCommand == null) _BackCommand = new DelegateCommand(OnBackCommand, () => CanBack)
                        .ObservesProperty(() => CanBack);
                return _BackCommand;
            }
        }

        public async void OnBackCommand()
        {
            await Navigation.NavigateBack(MyRegion);
        }
        #endregion

        #endregion

        private ServiceCollectionPropertyReadonly<RegionEntry> _history;

        public RegionContainerVm()
        {
        }

        [InjectionMethod]
        public void Inject()
        {
        }

        protected async override Task OnInitialized(object param)
        {
            await base.OnInitialized(param);

            MyRegion = new Region().WithName("Inside Region Container");
            await Navigation.RegisterRegion(MyRegion);

            _history = await Navigation.HistoryOf(MyRegion);
            await _history.Changed.Subscribe(this, args =>
            {
                throw new NotImplementedException();
//                CanBack = args.Last().CurrentItems.Any();
                return Tasks.Empty;
            });
        }

        protected async override Task OnClearing()
        {
            await base.OnClearing();
            await _history.Changed.Unsubscribe(this);
            await Navigation.UnregisterRegion(MyRegion);
        }



    }
}
