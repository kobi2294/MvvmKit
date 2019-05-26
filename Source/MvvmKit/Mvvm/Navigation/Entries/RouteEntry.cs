using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class RouteEntry : IEquatable<RouteEntry>
    {
        public Route Route { get; private set; }

        // expected to be:
        // For parameterless route: null
        // For fixed parameter route: the fixed parameter
        // For variant prameter route: the parameter that was supplied on routing
        public object Parameter { get; private set; }

        public static RouteEntry Empty { get; } = new RouteEntry(null, null);

        #region ctor and factory

        private RouteEntry(Route route, object parameter = null)
        {
            Route = route;
            Parameter = parameter;
        }

        public static RouteEntry Create(Route route, object parameter = null)
        {
            if (route == null) return Empty;

            switch (route.ParameterMode)
            {
                case RouteParameterMode.None:
                    return new RouteEntry(route);
                case RouteParameterMode.Fixed:
                    return new RouteEntry(route, route.Parameter);
                case RouteParameterMode.Variant:
                    return new RouteEntry(route, parameter);
                default:
                    throw new InvalidOperationException("Route has an invalid parameter mode");
            }
        }

        #endregion

        #region Comparing

        public override bool Equals(object obj)
        {
            if (!(obj is RouteEntry)) return false;
            return this == (RouteEntry)obj;
        }

        public bool Equals(RouteEntry other)
        {
            return this == other;
        }

        public static bool operator ==(RouteEntry rs1, RouteEntry rs2)
        {
            var isnull1 = ReferenceEquals(rs1, null);
            var isnull2 = ReferenceEquals(rs2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return (rs1.Route == rs2.Route) && (rs1.Parameter == rs2.Parameter);
        }

        public static bool operator !=(RouteEntry rs1, RouteEntry rs2)
        {
            return !(rs1 == rs2);
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(Route, Parameter);
        }

        #endregion

        public override string ToString()
        {
            if (this == Empty) return "Empty Route Entry";
            var res = Route.Name;
            if (Route.ParameterMode == RouteParameterMode.Variant)
                res = $"{res} ({Parameter})";
            return res;
        }
    }
}
