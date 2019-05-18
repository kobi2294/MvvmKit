using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class Route
    {
        public Type ViewModelType { get; internal set; }

        public object Parameter { get; internal set; }

        public Type ParameterType { get; internal set; }

        public string Name { get; internal set; }

        public object Key { get; internal set; }

        public RouteType RouteType { get; internal set; }

        public Router Owner { get; internal set; }

        public bool IsMatchingEntry(NavigationEntry entry)
        {
            var isMatchingViewModelType = entry.ViewModelType == ViewModelType;

            if (!isMatchingViewModelType) return false;

            if (RouteType == RouteType.FixedPerameter) return entry.Parameter == Parameter;

            if (RouteType == RouteType.Parameterless) return entry.Parameter == null;

            return true;
        }

        public static bool operator== (Route r1, Route r2)
        {
            var isnull1 = ReferenceEquals(r1, null);
            var isnull2 = ReferenceEquals(r2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return (r1.ViewModelType == r2.ViewModelType)
                && (r1.Key == r2.Key)
                && (r1.Parameter == r2.Parameter)
                && (r1.RouteType == r2.RouteType)
                && (r1.ParameterType == r2.ParameterType);
        }

        public static bool operator!= (Route r1, Route r2)
        {
            return !(r1 == r2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Route)) return false;
            return this == (Route)obj;
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(ViewModelType, Key, Parameter, ParameterType, RouteType);
        }
    }


}
