using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class NavigationEntry : IEquatable<NavigationEntry>
    {
        public Type ViewModelType { get; private set; }

        public Func<Task<ComponentBase>> Factory { get; private set; }

        public object Parameter { get; private set; }

        private NavigationEntry(Type viewModelType, object parameter = null, Func<Task<ComponentBase>> factory = null)
        {
            ViewModelType = viewModelType;
            Parameter = parameter;
            Factory = factory;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is NavigationEntry)) return false;

            return this == (NavigationEntry)obj;
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(ViewModelType, Parameter, Factory);
        }

        public override string ToString()
        {
            if (this == Empty) return "Empty Navigation Entry";
            return $"{this.ViewModelType.Name} ({Parameter})";
        }

        public bool Equals(NavigationEntry other)
        {
            return this == other;
        }



        public static NavigationEntry Create<T>(object param = null)
            where T : ComponentBase
        {
            return new NavigationEntry(typeof(T), param);
        }

        public static NavigationEntry Create(Type viewmodelType, object param)
        {
            if (!typeof(ComponentBase).IsAssignableFrom(viewmodelType))
                throw new ArgumentException(
                    $"Type must be a class that is derived from {nameof(ComponentBase)}",
                    nameof(viewmodelType));

            return new NavigationEntry(viewmodelType, param);
        }

        public static NavigationEntry CreateWithFactory(Func<Task<ComponentBase>> factory, object param)
        {
            return new NavigationEntry(typeof(ComponentBase), param, factory);
        }

        public static NavigationEntry Empty { get; } = new NavigationEntry(null, null);

        public static bool operator ==(NavigationEntry ne1, NavigationEntry ne2)
        {
            var isnull1 = ReferenceEquals(ne1, null);
            var isnull2 = ReferenceEquals(ne2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return (ne1.ViewModelType == ne2.ViewModelType) && (ne1.Parameter == ne2.Parameter) && (ne1.Factory == ne2.Factory);
        }

        public static bool operator !=(NavigationEntry ne1, NavigationEntry ne2)
        {
            return !(ne1 == ne2);
        }


    }
}
