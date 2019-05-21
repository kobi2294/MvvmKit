using $safeprojectname$.Components.Shell;
using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public class Bootstrapper : BootstrapperBase
    {
        protected override async Task ConfigureContainerOverride()
        {
            await base.ConfigureContainerOverride();
            RegionsService.RegisterStaticRegions(typeof(GlobalRegions));
            RoutersService.RegisterStaticRouters(typeof(GlobalRouters));
        }

        protected override async Task InitializeShellOverride()
        {
            await base.InitializeShellOverride();

            await RegionsService[GlobalRegions.ShellWindow].NavigateTo<ShellVm>();
        }
    }
}
