using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.RxMvvm
{
    public class ItemVm: BindableBase
    {
        private ItemModel _model;

        #region Properties

        private string _Uid;
        public string Uid { get { return _Uid; } set { SetProperty(ref _Uid, value); } }

        private string _DisplayName;
        public string DisplayName { get { return _DisplayName; } set { SetProperty(ref _DisplayName, value); } }

        private string _Category;
        public string Category { get { return _Category; } set { SetProperty(ref _Category, value); } }

        #endregion

        public ItemVm ReadModel(ItemModel model)
        {
            _model = model;
            Uid = model.Uid;
            DisplayName = model.DisplayName;
            Category = model.Category;
            Console.WriteLine($"updated model. uid: {Uid}, displayName: {DisplayName}, category: {Category}");
            return this;
        }


        protected override void OnDisposed()
        {
            base.OnDisposed();
            Console.WriteLine($"Disposing ItemVm uid: {Uid}");
        }

    }
}
