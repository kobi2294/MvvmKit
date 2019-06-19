using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKitAppSample.Components.NumberDialog
{
    public class NumberDialogVm : DialogBase<int>
    {
        #region Properties

        private int _Value;
        public int Value { get { return _Value; } set { SetProperty(ref _Value, value); } }


        #endregion

        public NumberDialogVm()
        {
            ValueOnCancel = -1;
            ReturnDefaultOnCancel = true;
        }

        [InjectionMethod]
        public void Inject()
        {
        }

        protected override async Task OnInitialized(object param)
        {
            await base.OnInitialized(param);
            if (param is int)
                Value = (int)param;
        }

    }
}
