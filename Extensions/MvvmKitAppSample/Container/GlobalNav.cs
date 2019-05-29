using MvvmKitAppSample.Components.Shell;
using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmKitAppSample.Components.PageOne;
using MvvmKitAppSample.Components.PageTwo;

namespace MvvmKitAppSample
{
    public static class GlobalNav
    {
        public enum ShellRoutes { Shell };

        public static Region ShellWindow { get; } = new Region()
            .Add(Route.To<ShellVm>(ShellRoutes.Shell))
            .Add(new OpenWindowRegionBehavior());

        // Add your global Regions, and routes, here
        public enum MainRoutes { One, Two };

        public static Region Main { get; } = new Region()
            .Add(Route.To<PageOneVm>(MainRoutes.One))
            .Add(Route.To<PageTwoVm>(MainRoutes.Two));

        public static Region Gain { get; } = new Region()
            .Add(Route.To<PageOneVm>(MainRoutes.One))
            .Add(Route.To<PageTwoVm>(MainRoutes.Two));

    }
}
