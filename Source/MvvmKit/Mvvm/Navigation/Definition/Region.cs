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
            Routes.Add(route);
            return this;
        }

        public override string ToString()
        {
            return $"Region ({Name}), Routes: {Routes.Count}, Behaviors: {Behaviors.Count}";
        }
    }
}
