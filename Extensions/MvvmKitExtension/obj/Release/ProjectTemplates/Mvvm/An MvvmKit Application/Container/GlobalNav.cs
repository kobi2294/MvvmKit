using $safeprojectname$.Components.Shell;
using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public static class GlobalNav
    {
        public enum ShellRoutes { Shell };

        public static Region ShellWindow { get; } = new Region()
            .Add(Route.To<ShellVm>(ShellRoutes.Shell))
            .Add(new OpenWindowRegionBehavior());

        // Add your global Regions, and routes, here
    }
}
