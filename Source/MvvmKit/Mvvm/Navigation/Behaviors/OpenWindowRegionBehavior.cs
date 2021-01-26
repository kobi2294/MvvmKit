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
        private HashSet<Window> _myWindows = new HashSet<Window>();
        private Action _onClosing = null;

        public Style WindowStyle { get; private set; }

        public OpenWindowRegionBehavior WithStyle(Style style)
        {
            if (style.TargetType != typeof(Window))
                throw new ArgumentException("Style must have a target type set to Window", nameof(style));

            WindowStyle = style;
            return this;
        }

        public OpenWindowRegionBehavior WithOnClosing(Action onClosing)
        {
            _onClosing = onClosing;
            return this;
        }

        private Task _ensureWindowOpen(RegionService service)
        {
            if (!service.Hosts.OfType<Window>().Any())
            {
                var w = new Window();

                w.Unloaded += (s, e) =>
                {
                    var action = _onClosing;
                    _onClosing = null;
                    action?.Invoke();
                };

                Application.Current.Exit += (s, e) =>
                {
                    var action = _onClosing;
                    _onClosing = null;
                    action?.Invoke();
                };

                _myWindows.Add(w);
                w.Style = WindowStyle;
                RegionHost.SetRegion(w, service.Region);
                w.Show();
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
            }
            else
            {
                await _ensureWindowOpen(service);
            }
        }

        protected override async Task BeforeNavigationOverride(RegionService service)
        {
            await base.BeforeNavigationOverride(service);
            await _invalidate(service);
        }
    }
}
