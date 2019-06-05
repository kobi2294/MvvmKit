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

        public PageTwoVm()
        {
        }

        [InjectionMethod]
        public void Inject()
        {
        }

    }
}
