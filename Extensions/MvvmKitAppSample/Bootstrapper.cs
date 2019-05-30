using MvvmKitAppSample.Components.Shell;
using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmKitAppSample.Services;

namespace MvvmKitAppSample
{
    public class Bootstrapper : BootstrapperBase
    {
        protected override async Task ConfigureContainerOverride()
        {
            await base.ConfigureContainerOverride();
            await Navigation.RegisterStaticRegions(typeof(GlobalNav));
            RegisterService<IUiService, UiService>();
        }

        protected override async Task InitializeShellOverride()
        {
            await base.InitializeShellOverride();
            await Navigation.RouteTo(GlobalNav.ShellRoutes.Shell);
            await Navigation.RouteTo(GlobalNav.Main, GlobalNav.MainRoutes.One);
            await Navigation.RouteTo(GlobalNav.Gain, GlobalNav.MainRoutes.Two);
        }
    }
}
