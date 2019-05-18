using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class Router
    {
        private List<Route> _routes;
        private Dictionary<object, Route> _byKey;
        public Region Region {get; private set;}

        public IEnumerable<Route> Routes {  get { return _routes.ToList(); } }
        public IEnumerable<Route> FixedParameterRoutes { get { return Routes.Where(x => x.RouteType == RouteType.FixedPerameter); } }
        public IEnumerable<Route> ParameterlessRoutes { get { return Routes.Where(x => x.RouteType == RouteType.Parameterless); } }
        public IEnumerable<Route> VariantParameterRoutes { get { return Routes.Where(x => x.RouteType == RouteType.VariantParameter); } }

        public int IndexOf(Route route)
        {
            return _routes.IndexOf(route);
        }

        public int IndexOf(object key)
        {
            var route = _byKey[key];
            return IndexOf(route);
        }

        public Route this[object key]
        {
            get
            {
                return _byKey[key];
            }
        }

        internal RoutersService Owner { get; set; }        

        internal Router(IEnumerable<Route> routes, Region region)
        {
            _routes = routes.ToList();
            _byKey = _routes.ToDictionary(x => x.Key);
            Region = region;

            foreach (var route in _routes)
            {
                route.Owner = this;
            }
        }

        public Route FindMatchingRoute(NavigationEntry entry)
        {
            // we search for a match on the fixed routes first, then the parameterless, and lasty the variant
            var routesToSearch = FixedParameterRoutes.Concat(ParameterlessRoutes).Concat(VariantParameterRoutes);

            return routesToSearch.FirstOrDefault(r => r.IsMatchingEntry(entry));
        }


        public static RouterBuilder For(Region region)
        {
            return new RouterBuilder(region);
        }

        public static implicit operator Router(RouterBuilder builder)
        {
            return builder.Build();
        }

    }
}
