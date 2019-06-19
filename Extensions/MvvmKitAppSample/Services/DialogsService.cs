using MvvmKit;
using MvvmKitAppSample.Components.MessageBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKitAppSample.Services
{
    public class DialogsService : UiServiceBase
    {
        private NavigationService _navigation;
        private Region _region;


        protected override async Task OnInit()
        {
            await base.OnInit();

            // init logic here
            _region = new Region()
            .WithName("Dialogs")
            .Add(new ModalDialogWindowBehavior());

            await _navigation.Init();
            await _navigation.RegisterRegion(_region);
    }

    [InjectionMethod]
        public void Inject(NavigationService nav)
        {
            _navigation = nav;
        }

        public Task Message(string caption)
        {
            return _navigation.RunDialog<MessageBoxVm>(_region, caption);
        }
    }
}
