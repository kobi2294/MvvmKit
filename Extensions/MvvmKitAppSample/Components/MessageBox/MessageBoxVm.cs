using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKitAppSample.Components.MessageBox
{
    public class MessageBoxVm : DialogBase
    {
        #region Properties

        private string _Caption;
        public string Caption { get { return _Caption; } set { SetProperty(ref _Caption, value); } }

        #endregion

        public MessageBoxVm()
        {
        }

        [InjectionMethod]
        public void Inject()
        {
        }

        protected async override Task OnInitialized(object param)
        {
            await base.OnInitialized(param);
            if (param is string str)
            {
                Caption = str;
            }
        }

    }
}
