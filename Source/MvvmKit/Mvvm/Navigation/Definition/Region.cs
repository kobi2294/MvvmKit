using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class Region
    {
        public string Name { get; private set; }

        public HashSet<RegionBehavior> Behaviors { get; } = new HashSet<RegionBehavior>();

        public HashSet<Route> Routes { get; } = new HashSet<Route>();

        public IEnumerable<Route> AllRoutes => Routes.ToList();
        public IEnumerable<Route> FixedRoutes => Routes.Where(x => x.ParameterMode == RouteParameterMode.Fixed);
        public IEnumerable<Route> NoneRoutes => Routes.Where(x => x.ParameterMode == RouteParameterMode.None);
        public IEnumerable<Route> VariantRoutes => Routes.Where(x => x.ParameterMode == RouteParameterMode.Variant);


        public Region WithName(string name)
        {
            Name = name;
            return this;
        }

        public Region Add(RegionBehavior behavior)
        {
            Behaviors.Add(behavior);
            return this;
        }

        public Region Add(Route route)
        {
            // critical to maintain the order here, because region is part of the hashcode of the route, 
            // so all properties must be set before adding it to the hashset
            route.Region = this;
            Routes.Add(route);
            return this;
        }

        public override string ToString()
        {
            return $"Region ({Name}), Routes: {Routes.Count}, Behaviors: {Behaviors.Count}";
        }
    }
}
