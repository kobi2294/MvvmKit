using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class RegionEntry : IEquatable<RegionEntry>
    {
        public Type ViewModelType { get; private set; }

        public object Parameter { get; set; }

        public static RegionEntry Empty { get; } = new RegionEntry(null, null);

        #region ctor and factory

        private RegionEntry(Type viewModelType, object parameter = null)
        {
            ViewModelType = viewModelType;
            Parameter = parameter;
        }

        public static RegionEntry Create(Type viewmodelType, object param)
        {
            if (!typeof(ComponentBase).IsAssignableFrom(viewmodelType))
                throw new ArgumentException(
                    $"Type must be a class that is derived from {nameof(ComponentBase)}",
                    nameof(viewmodelType));

            return new RegionEntry(viewmodelType, param);
        }

        public static RegionEntry Create<T>(object param = null)
            where T : ComponentBase
        {
            return new RegionEntry(typeof(T), param);
        }

        #endregion

        #region Comparing
        public override bool Equals(object obj)
        {
            if (!(obj is RegionEntry)) return false;

            return this == (RegionEntry)obj;
        }

        public bool Equals(RegionEntry other)
        {
            return this == other;
        }

        public static bool operator ==(RegionEntry rs1, RegionEntry rs2)
        {
            var isnull1 = ReferenceEquals(rs1, null);
            var isnull2 = ReferenceEquals(rs2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return (rs1.ViewModelType == rs2.ViewModelType)
                && (rs1.Parameter == rs2.Parameter);
        }

        public static bool operator !=(RegionEntry rs1, RegionEntry rs2)
        {
            return !(rs1 == rs2);
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(ViewModelType, Parameter);
        }

        #endregion

        public bool IsMatchingRoute(Route route)
        {
            var isMatchingViewModelType = ViewModelType == route.ViewModelType;

            if (!isMatchingViewModelType) return false;

            if (route.ParameterMode == RouteParameterMode.Fixed) return Parameter == route.Parameter;

            if (route.ParameterMode == RouteParameterMode.None) return Parameter == null;

            return true;
        }

        public Route FindMatchingRoute(Region region)
        {
            var routesToSreach = region.FixedRoutes.Concat(region.NoneRoutes).Concat(region.VariantRoutes);
            return routesToSreach.FirstOrDefault(r => IsMatchingRoute(r));
        }



        public override string ToString()
        {
            if (this == Empty) return "Empty Region Entry";
            return $"{this.ViewModelType.Name} ({Parameter})";
        }
    }
}
