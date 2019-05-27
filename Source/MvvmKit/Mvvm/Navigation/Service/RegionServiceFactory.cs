using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class RegionServiceFactory
    {
        private IResolver _resolver;

        public RegionServiceFactory(IResolver resolver)
        {
            _resolver = resolver;
        }

        public RegionService CreateService(Region region, NavigationService owner)
        {
            return new RegionService(region, owner, _resolver);
        }
    }
}
