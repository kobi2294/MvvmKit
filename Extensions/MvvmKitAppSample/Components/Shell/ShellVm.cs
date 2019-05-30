using MvvmKit;
using MvvmKitAppSample.Components.PageOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKitAppSample.Components.Shell
{
    public class ShellVm : ComponentBase
    {
        #region Properties

        private string _Title;
        public string Title { get { return _Title; } set { SetProperty(ref _Title, value); } }


        private Region _MyRegion;
        public Region MyRegion { get { return _MyRegion; } set { SetProperty(ref _MyRegion, value); } }


        #endregion

        #region Commands

        #endregion

        public ShellVm()
        {
            Title = "Hello MvvmKit Runtime";
            MyRegion = new Region();
        }

        protected override async Task OnInitialized(object param)
        {
            await base.OnInitialized(param);

            await Navigation.RegisterRegion(MyRegion);
            await Navigation.NavigateTo<PageOneVm>(MyRegion);
        }

        [InjectionMethod]
        public void Inject()
        {

        }
    }
}
