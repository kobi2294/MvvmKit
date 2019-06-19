using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKitAppSample.Components.RegionContainer
{
    public class RegionContainerVm : ComponentBase
    {
        #region Properties

        private Region _MyRegion;
        public Region MyRegion { get { return _MyRegion; } set { SetProperty(ref _MyRegion, value); } }

        #endregion

        public RegionContainerVm()
        {
        }

        [InjectionMethod]
        public void Inject()
        {
        }

        protected async override Task OnInitialized(object param)
        {
            await base.OnInitialized(param);
            MyRegion = new Region().WithName("My Region");
            await Navigation.RegisterRegion(MyRegion);
        }

        protected async override Task OnClearing()
        {
            await base.OnClearing();
            await Navigation.UnregisterRegion(MyRegion);
        }



    }
}
