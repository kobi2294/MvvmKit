using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectiveResourcesDemo.SelectManyDemo
{
    public class ItemVm: BindableBase
    {
        #region Properties

        private string _Uid;
        public string Uid { get { return _Uid; } set { SetProperty(ref _Uid, value); } }

        private string _DisplayName;
        public string DisplayName { get { return _DisplayName; } set { SetProperty(ref _DisplayName, value); } }

        #endregion

        public ItemVm ReadModel(ItemModel model)
        {
            Uid = model.Uid;
            DisplayName = model.DisplayName;
            return this;
        }
    }
}
