using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKitAppSample.Components.PageOne
{
    public class ItemVm : BindableBase
    {

        private string _Value;
        public string Value { get { return _Value; } set { SetProperty(ref _Value, value); } }

        #region Remove Command

        private DelegateCommand _RemoveCommand;
        public DelegateCommand RemoveCommand
        {
            get
            {
                if (_RemoveCommand == null) _RemoveCommand = new DelegateCommand(OnRemoveCommand);
                return _RemoveCommand;
            }
        }


        public async void OnRemoveCommand()
        {
            await _parent.OnChildRemove(this);
        }

        #endregion

        private PageOneVm _parent;


        public Task<ItemVm> ReadModel(string model, PageOneVm parent)
        {
            Value = model;
            _parent = parent;
            return Task.FromResult(this);
        }
    }
}
