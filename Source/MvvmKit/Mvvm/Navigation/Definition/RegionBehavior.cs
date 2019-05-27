using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class RegionBehavior
    {
        internal Task BeforeNavigation(RegionService service)
        {
            return BeforeNavigationOverride(service);
        }

        protected virtual Task BeforeNavigationOverride(RegionService service)
        {
            return Tasks.Empty;
        }

        internal Task AfterNavigation(RegionService service)
        {
            return AfterNavigationOverride(service);
        }

        protected virtual Task AfterNavigationOverride(RegionService service)
        {
            return Tasks.Empty;
        }

    }
}
