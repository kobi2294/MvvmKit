using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class RegionsService
    {
        private RegionManagerFactory _factory;

        private Dictionary<Region, RegionManager> _managers { get; } = new Dictionary<Region, RegionManager>();

        public RegionsService(RegionManagerFactory factory)
        {
            _factory = factory;
        }

        public RegionManager this[Region region]
        {
            get
            {
                return _managers[region];
            }
        }

        /// <summary>
        /// Registers a <see cref="Region">Region</see> under the service. 
        /// Once a region was registered to it, the service manages navigation and view hosting for that region.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the region is already registerd to another service</exception>
        /// <param name="region">The region to register</param>
        public void RegisterRegion(Region region)
        {
            if (region.Owner != null)
                throw new InvalidOperationException("The region already belongs to another RegionsService");

            var manager = _factory.CreateManager(region, this);
            region.Owner = this;
            _managers.Add(region, manager);
        }

        /// <summary>
        /// Enumerates all the static properties of type <see cref="Region">Region</see> of this type and registers them.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterStaticRegions(Type type)
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

                RegisterRegion(region);
            }


        }

    }
}
