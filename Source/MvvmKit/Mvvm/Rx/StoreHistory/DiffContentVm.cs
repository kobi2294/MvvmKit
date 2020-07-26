using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Mvvm.Rx.StoreHistory
{
    public class DiffContentVm : BindableBase
    {
        #region Properties

        private string _OldState;
        public string OldState { get { return _OldState; } set { SetProperty(ref _OldState, value); } }

        private string _NewState;
        public string NewState { get { return _NewState; } set { SetProperty(ref _NewState, value); } }

        #endregion
    }
}
