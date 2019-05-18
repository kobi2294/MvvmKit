using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class RouterManager
    {
        public AsyncEvent Navigated { get; } = new AsyncEvent();

        private IResolver _resolver;

        public RoutersService OwnerService { get; }

        public Router Router { get; }

        public RouterEntry CurrentRoute { get; private set; }

        public RegionManager RegionManager { get; private set; }

        public RouterManager(Router router, RoutersService owner, IResolver resolver)
        {
            CurrentRoute = RouterEntry.Empty;
            _resolver = resolver;
            OwnerService = owner;
            Router = router;

            // register to region owner
            _registerToEvents();
        }

        private void _registerToEvents()
        {
            RegionManager = Router.Region.Owner[Router.Region];
            RegionManager.Navigated.Subscribe(this, _onRegionNavigation);
            _invalidateCurrentRouteEntry();
        }

        private async Task _onRegionNavigation()
        {
            _invalidateCurrentRouteEntry();
            await Navigated.Invoke();
        }

        private void _invalidateCurrentRouteEntry()
        {
            var manager = Router.Region.Owner[Router.Region];
            var entry = manager.CurrentNavigation;

            var route = Router.FindMatchingRoute(entry);

            if (route == null)
                CurrentRoute = RouterEntry.Empty;
            else
                CurrentRoute = RouterEntry.Create(route, entry.Parameter);
        }

        public async Task Navigate(Route route, object parameter = null)
        {
            if (route.Owner != Router)
                throw new InvalidOperationException("route does not belong to the current router");

            var manager = Router.Region.Owner[Router.Region];
            var vmType = route.ViewModelType;

            if (route.RouteType != RouteType.VariantParameter)
                parameter = route.Parameter;

            await manager.NavigateTo(route.ViewModelType, parameter);
        }

        public async Task Navigate(object key, object parameter = null)
        {
            var route = Router[key];
            await Navigate(route, parameter);
        }
    }
}
