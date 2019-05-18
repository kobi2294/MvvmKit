using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    /// <summary>
    /// Represents a possible current state of a router
    /// </summary>
    public class RouterEntry
    {
        public Route Route { get; private set; }

        // expected to be:
        // For parameterless route: null
        // For fixed parameter route: the fixed parameter
        // For variant prameter route: the parameter that was supplied on routing
        public object Parameter { get; private set; }

        private RouterEntry(Route route, object parameter = null)
        {
            Route = route;
            Parameter = parameter;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RouterEntry)) return false;
            return this == (RouterEntry)obj;
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(Route, Parameter);
        }

        public override string ToString()
        {
            if (this == Empty) return "Empty Route Entry";
            var res = Route.Name;
            if (Route.RouteType == RouteType.VariantParameter)
                res = $"{res} ({Parameter})";
            return res;
        }

        public static RouterEntry Empty { get; } = new RouterEntry(null, null);

        public static bool operator== (RouterEntry re1, RouterEntry re2)
        {
            var isnull1 = ReferenceEquals(re1, null);
            var isnull2 = ReferenceEquals(re2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return (re1.Route == re2.Route) && (re1.Parameter == re2.Parameter);
        }

        public static bool operator!= (RouterEntry re1, RouterEntry re2)
        {
            return !(re1 == re2);
        }


        public static RouterEntry Create(Route route, object parameter = null)
        {
            if (route == null) return Empty;

            switch (route.RouteType)
            {
                case RouteType.Parameterless:
                    return new RouterEntry(route);
                case RouteType.FixedPerameter:
                    return new RouterEntry(route, route.Parameter);
                case RouteType.VariantParameter:
                    return new RouterEntry(route, parameter);
                default:
                    throw new InvalidOperationException("route has not valid router type");
            }
        }
    }
}
