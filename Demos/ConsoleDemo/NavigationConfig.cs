using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo
{
    public static class NavigationConfig
    {
        public enum MainRoutes { One, Two, Three };
        public static Region Main { get; } = new Region()
            .WithName("Main")
            .Add(Route.To<ComponentBase>(MainRoutes.One).WithoutParam())
            .Add(Route.To<ComponentBase>(MainRoutes.Two).WithParam<int>())
            .Add(Route.To<ComponentBase>(MainRoutes.Three).WithParam("Fourteen"));
    }
}
