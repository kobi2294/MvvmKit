﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class NavigationService : UiServiceBase
    {
        private RegionServiceFactory _factory;
        private Dictionary<Region, RegionService> _serviceByRegion { get; } = new Dictionary<Region, RegionService>();
        private Dictionary<Route, RegionService> _serviceByRoute { get; } = new Dictionary<Route, RegionService>();
        private EditableLookup<object, Route> _routeByKey { get; } = new EditableLookup<object, Route>();

        private RegionService _serviceFor(Region region)
        {
            return _serviceByRegion.GetOrDefault(region);
        }

        private RegionService _serviceFor(Route route)
        {
            return _serviceByRoute.GetOrDefault(route);
        }

        public NavigationService(RegionServiceFactory factory)
        {
            _factory = factory;
        }

        public RegionService this[Region region]
        {
            get
            {
                return _serviceFor(region);
            }
        }

        public RegionService this[Route route]
        {
            get
            {
                return _serviceFor(route);
            }
        }

        private void _registerRegion(Region region)
        {
            if (_serviceByRegion.ContainsKey(region))
            {
                throw new InvalidOperationException($"The region '{region}' is already registered in this service");
            }

            var service = _factory.CreateService(region, this);
            _serviceByRegion.Add(region, service);

            foreach (var route in region.Routes)
            {
                _serviceByRoute.Add(route, service);
                _routeByKey.Add(route.Key, route);
            }
        }

        private Task _unregisterRegion(Region region)
        {
            if (!_serviceByRegion.ContainsKey(region))
            {
                throw new InvalidOperationException($"The region '{region}' is not registered in this service");
            }

            var service = _serviceByRegion[region];

            // remove region and routes from registry
            _serviceByRegion.Remove(region);
            foreach (var route in region.Routes)
            {
                _serviceByRoute.Remove(route);
                _routeByKey.Remove(route.Key, route);
            }

            return Task.CompletedTask;
        }

        private void _registerStaticRegions(Type type)
        {
            var properties = type
               .GetProperties(BindingFlags.Public | BindingFlags.Static)
               .ToArray();

            foreach (var pi in properties)
            {
                var region = pi.GetValue(null) as Region;

                // if the property is empty, create a region and place in it
                if (region == null)
                {
                    region = new Region();
                    pi.SetValue(null, region);
                }

                // if it does not have a name, give it the name of the property
                if (string.IsNullOrWhiteSpace(region.Name))
                {
                    region.WithName(pi.Name);
                }

                _registerRegion(region);
            }
        }


        public Task RegisterRegion(Region region)
        {
            return Run(() =>
            {
                _registerRegion(region);
            });
        }

        public Task RegisterStaticRegions(Type type)
        {
            return Run(() => _registerStaticRegions(type));
        }

        public async Task UnregisterRegion(Region region)
        {
            var service = _serviceByRegion[region];

            await Run(async () => await _unregisterRegion(region));

            // finalize region service - we do it outside the "Run" zone to avoid deadlocks
            await service.OnUnregistering();

        }

        public async Task<ComponentBase> NavigateTo(Region region, Type t, object param = null)
        {
            var service = _serviceFor(region);
            return await service.NavigateTo(t, param);
        }

        public async Task<TVM> NavigateTo<TVM>(Region region, object param = null)
            where TVM: ComponentBase
        {
            var service = _serviceFor(region);
            return await service.NavigateTo<TVM>(param);
        }

        public async Task<ComponentBase> NavigateBack(Region region)
        {
            var service = _serviceFor(region);
            return await service.NavigateBack();
        }

        public async Task Clear(Region region)
        {
            var service = _serviceFor(region);
            await service.Clear();
        }

        public  async Task<T> RunDialog<TVM, T>(Region region, object param = null)
            where TVM: DialogBase<T>
        {
            var service = _serviceFor(region);
            return await service.RunDialog<TVM, T>(param);
        }

        public async Task RunDialog<TVM>(Region region, object param = null)
            where TVM : DialogBase
        {
            var service = _serviceFor(region);
            await service.RunDialog<TVM>(param);
        }

        public async Task<ComponentBase> RouteTo(Route route, object param = null)
        {
            var service = _serviceFor(route);
            return await service.RouteTo(route, param);
        }

        public async Task<ComponentBase> RouteTo(Region region, object key, object param = null)
        {
            var service = _serviceFor(region);
            return await service.RouteTo(key, param);
        }

        public async Task<ComponentBase> RouteTo(object key, object param = null)
        {
            var route = await Run(() =>
            {
                var routes = _routeByKey[key];
                if (routes.Count() > 1)
                    throw new InvalidOperationException("There are more than a single route with the same key, please specify the region");

                if (routes.Count() == 0)
                    throw new InvalidOperationException("There are no routes with the privided key");

                return routes.Single();
            });
            var service = _serviceFor(route.Region);
            return await service.RouteTo(route, param);
        }

        public async Task DestroyEntryStateWhere(Region region, Func<RegionEntry, bool> entryPicker)
        {
            var service = _serviceFor(region);
            await service.DestroyEntryStateWhere(entryPicker);
        }

        public Task<ServicePropertyReadonly<ComponentBase>> CurrentViewModelAt(Region region)
        {
            return Run(() =>
            {
                var service = _serviceByRegion[region];
                return service.CurrentViewModel;
            });
        }

        public Task<ServicePropertyReadonly<RegionEntry>> CurrentRegionEntryAt(Region region)
        {
            return Run(() =>
            {
                var service = _serviceByRegion[region];
                return service.CurrentRegionEntry;
            });
        }

        public Task<ServicePropertyReadonly<RouteEntry>> CurrentRouteEntry(Region region)
        {
            return Run(() =>
            {
                var service = _serviceByRegion[region];
                return service.CurrentRouteEntry;
            });
        }

        public Task<ServiceCollectionPropertyReadonly<RegionEntry>> HistoryOf(Region region)
        {
            return Run(() =>
            {
                var service = _serviceByRegion[region];
                return service.History;
            });
        }
    }
}
