using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Mvvm.Rx.StoreHistory
{
    public class EnsureItemVm: BindableBase
    {
        #region Properties

        private string _Caption;
        public string Caption { get { return _Caption; } set { SetProperty(ref _Caption, value); } }


        private string _EntityType;
        public string EntityType { get { return _EntityType; } set { SetProperty(ref _EntityType, value); } }


        #endregion


        public EnsureItemVm ReadModel(EnsureHistoryItem model)
        {
            Caption = model.EnsureMethod;
            EntityType = model.Context.Entity.GetType().GetTypeName();
            return this;
        }
    }
}
