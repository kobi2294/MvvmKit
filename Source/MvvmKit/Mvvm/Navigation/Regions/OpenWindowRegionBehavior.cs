using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmKit
{
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

        protected override async Task BeforeNavigationOverride(RegionManager manager)
        {
            await base.BeforeNavigationOverride(manager);

            if (!manager.Hosts.OfType<Window>().Any())
            {
                var w = new Window();
                w.Style = WindowStyle;
                RegionHost.SetRegion(w, manager.Region);
                w.Show();
            }
        }
    }
}
