using MvvmKit;
using MvvmKitAppSample.Components.NumberDialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKitAppSample.Components.PageTwo
{
    public class PageTwoVm : ComponentBase
    {
        #region Properties

        private int _LatestValue;
        public int LatestValue { get { return _LatestValue; } set { SetProperty(ref _LatestValue, value); } }

        #endregion

        #region Commands

        #region GetNewValue Command

        private DelegateCommand _GetNewValueCommand;
        public DelegateCommand GetNewValueCommand
        {
            get
            {
                if (_GetNewValueCommand == null) _GetNewValueCommand = new DelegateCommand(OnGetNewValueCommand);
                return _GetNewValueCommand;
            }
        }

        public async void OnGetNewValueCommand()
        {
            LatestValue = await Navigation.RunDialog<NumberDialogVm, int>(GlobalNav.ModalDialog, LatestValue);
        }


        #endregion

        #endregion  

        int _myNum;

        public PageTwoVm()
        {
            var rand = new Random();
            _myNum = rand.Next(100);
        }

        [InjectionMethod]
        public void Inject()
        {
        }

        protected override Task OnNewState()
        {
            LatestValue = 0;
            return Task.CompletedTask;
        }

        protected async override Task OnSaveState(StateSaver state)
        {
            await base.OnSaveState(state);
            state.Save(() => LatestValue);
            state.Save(() => _myNum);
            state.Set("Kobi", 42);
        }

        protected async override Task OnRestoreState(StateRestorer state)
        {
            await base.OnRestoreState(state);
            var kobi = state.Get<int>("Kobi");
        }

    }
}
