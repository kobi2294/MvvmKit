using MvvmAppSample.Components.Shell;
using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmAppSample
{
    public class Bootstrapper : BootstrapperBase
    {
        protected override async Task ConfigureContainerOverride()
        {
            await base.ConfigureContainerOverride();
            await Navigation.RegisterStaticRegions(typeof(GlobalNav));

        }

        protected override async Task InitializeShellOverride()
        {
            await base.InitializeShellOverride();
            await Navigation.RouteTo(GlobalNav.ShellRoutes.Shell);
        }
    }
}
