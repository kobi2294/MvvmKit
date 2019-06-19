using MvvmKitAppSample.Components.Shell;
using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmKitAppSample.Services;
using Unity;
using System.Diagnostics;
using System.Threading;

namespace MvvmKitAppSample
{
    public class Bootstrapper : BootstrapperBase
    {
        protected override async Task ConfigureContainerOverride()
        {
            await base.ConfigureContainerOverride();
            await Navigation.RegisterStaticRegions(typeof(GlobalNav));
            RegisterService<IUiService, UiService>();
            RegisterService<BackgroundService>();
            RegisterService<IBgService2, BgService2>();
            RegisterService<IBgService1, BgService1>();
            RegisterService<DialogsService>();
        }

        protected override async Task InitializeShellOverride()
        {
            await base.InitializeShellOverride();

            var bgs = Container.Resolve<BackgroundService>();
            var uis = Container.Resolve<IUiService>();

            await Navigation.RouteTo(GlobalNav.ShellRoutes.Shell);
            await Navigation.RouteTo(GlobalNav.Main, GlobalNav.MainRoutes.One);
            await Navigation.RouteTo(GlobalNav.Gain, GlobalNav.MainRoutes.Two);
        }
    }
}
