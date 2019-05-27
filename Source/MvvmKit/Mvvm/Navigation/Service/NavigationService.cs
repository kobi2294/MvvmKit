using System;
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


        public NavigationService(RegionServiceFactory factory)
        {
            _factory = factory;
        }

        public Task<RegionService> For(Region region)
        {
            return Run(() =>
            {
                return _serviceByRegion[region];
            });
        }

        public Task<RegionService> For(Route route)
        {
            return Run(() =>
            {
                return _serviceByRoute[route];
            });
        }

        private void _registerRegion(Region region)
        {
            if (_serviceByRegion.ContainsKey(region))
            {
                throw new InvalidOperationException("The region is already registered in this service");
            }

            var service = _factory.CreateService(region, this);
            _serviceByRegion.Add(region, service);

            foreach (var route in region.Routes)
            {
                _serviceByRoute.Add(route, service);
                _routeByKey.Add(route.Key, route);
            }
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
            }, true);
        }

        public Task RegisterStaticRegions(Type type)
        {
            return Run(() =>
            {
                _registerStaticRegions(type);
            }, true);
        }

        public Task<ComponentBase> NavigateTo(Region region, Type t, object param = null)
        {
            return Run(() =>
            {
                var service = _serviceByRegion[region];
                return service.NavigateTo(t, param);
            });
        }

        public Task<TVM> NavigateTo<TVM>(Region region, object param = null)
            where TVM: ComponentBase
        {
            return Run(() =>
            {
                var service = _serviceByRegion[region];
                return service.NavigateTo<TVM>(param);
            });
        }

        public Task Clear(Region region)
        {
            return Run(() =>
            {
                var service = _serviceByRegion[region];
                return service.Clear();
            });
        }

        public  Task<T> RunDialog<TVM, T>(Region region, object param = null)
            where TVM: DialogBase<T>
        {
            return Run(() =>
            {
                var service = _serviceByRegion[region];
                return service.RunDialog<TVM, T>(param);
            });
        }

        public  Task<ComponentBase> RouteTo(Route route, object param = null)
        {
            return Run(() =>
            {
                var service = _serviceByRoute[route];
                return service.RouteTo(route, param);

            });
        }

        public Task<ComponentBase> RouteTo(Region region, object key, object param = null)
        {
            return Run(() =>
            {
                var service = _serviceByRegion[region];
                return service.RouteTo(key, param);
            });
        }

        public Task<ComponentBase> RouteTo(object key, object param = null)
        {
            return Run(() =>
            {
                var routes = _routeByKey[key];
                if (routes.Count() > 1)
                    throw new InvalidOperationException("There are more than a single route with the same key, please specify the region");

                if (routes.Count() == 0)
                    throw new InvalidOperationException("There are no routes with the privided key");

                var route = routes.Single();
                var service = _serviceByRoute[route];
                return service.RouteTo(route, param);
            });

        }


    }
}
