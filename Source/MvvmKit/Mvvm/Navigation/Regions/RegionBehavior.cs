using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class RegionBehavior
    {
        protected virtual Task BeforeNavigationOverride(RegionManager manager)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterNavigationOverride(RegionManager manager)
        {
            return Task.CompletedTask;
        }

        internal async Task BeforeNavigation(RegionManager manager)
        {
            await BeforeNavigationOverride(manager);
        }

        internal async Task AfterNavigation(RegionManager manager)
        {
            await AfterNavigationOverride(manager);
        }
    }
}
