using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmKit
{
    /// <summary>
    /// A Behavior that verifies that there is a host of type Window for this region.
    /// <para>
    /// If during navigation a window host is not registered, it creates a new window and assings it as region host
    /// </para>
    /// </summary>
    public class OpenWindowRegionBehavior : RegionBehavior
    {
        public Style WindowStyle { get; private set; }

        public OpenWindowRegionBehavior WithStyle(Style style)
        {
            if (style.TargetType != typeof(Window))
                throw new ArgumentException("Style must have a target type set to Window", nameof(style));

            WindowStyle = style;
            return this;
        }

        protected override async Task BeforeNavigationOverride(RegionService service)
        {
            await base.BeforeNavigationOverride(service);

            if (!service.Hosts.OfType<Window>().Any())
            {
                var w = new Window();
                w.Style = WindowStyle;
                RegionHost.SetRegion(w, service.Region);
                w.Show();
            }
        }
    }
}
