using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class RegionManagerFactory
    {
        private IResolver _resolver;

        public RegionManagerFactory(IResolver resolver)
        {
            _resolver = resolver;
        }

        public RegionManager CreateManager(Region region, RegionsService owner)
        {
            return new RegionManager(region, owner, _resolver);
        }
    }
}
