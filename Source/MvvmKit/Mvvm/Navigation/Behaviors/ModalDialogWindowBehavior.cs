using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmKit
{
    /// <summary>
    /// A behavior that manages a modal dialog window for the region. It opens a modal window as host when the region is navigated into, 
    /// then closes it when the region is cleared
    /// </summary>
    public class ModalDialogWindowBehavior: RegionBehavior
    {
        private HashSet<Window> _myWindows = new HashSet<Window>();

        public Style WindowStyle { get; private set; }

        public SizeToContent SizeToContent { get; private set; } = SizeToContent.WidthAndHeight;
        public WindowStartupLocation WindowStartupLocation { get; private set; } = WindowStartupLocation.CenterOwner;

        public ModalDialogWindowBehavior()
        {
        }

        public ModalDialogWindowBehavior WithStyle(Style style)
        {
            if (style.TargetType != typeof(Window))
                throw new ArgumentException("Style must have a target type set to Window", nameof(style));

            WindowStyle = style;
            return this;
        }

        public ModalDialogWindowBehavior WithSizeToContent(SizeToContent size)
        {
            SizeToContent = size;
            return this;
        }

        public ModalDialogWindowBehavior WithStartupLocation(WindowStartupLocation location)
        {
            WindowStartupLocation = location;
            return this;
        }

        private Task _ensureWindowOpen(RegionService service)
        {
            if (!service.Hosts.OfType<Window>().Any())
            {
                var w = new Window();
                w.SizeToContent = SizeToContent;
                w.WindowStartupLocation = WindowStartupLocation;
                _myWindows.Add(w);
                w.Style = WindowStyle;
                RegionHost.SetRegion(w, service.Region);

                // simply calling ShowDialog will block the method, so e need to trick it...
                w.Dispatcher.BeginInvoke(new Action(() => {
                    // make sure window was not closed since dispatching
                    if (_myWindows.Contains(w))
                    {
                        w.ShowDialog();
                    }
                }));
            }

            return Tasks.Empty;
        }

        private Task _ensureWindowCloses(RegionService service)
        {
            var windows = service.Hosts.OfType<Window>();
            var mine = windows.Intersect(_myWindows).ToList();

            foreach (var win in mine)
            {
                win.Close();
                _myWindows.Remove(win);
            }
            return Tasks.Empty;
        }

        private async Task _invalidate(RegionService service)
        {
            var entry = await service.CurrentRegionEntry.Get();

            if (entry == RegionEntry.Empty)
            {
                await _ensureWindowCloses(service);
            } else
            {
                await _ensureWindowOpen(service);
            }
        }


        protected override async Task AfterNavigationOverride(RegionService service)
        {
            await base.AfterNavigationOverride(service);
            await _invalidate(service);
        }



    }
}
