using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class RouterManagerFactory
    {
        private IResolver _resolver;

        public RouterManagerFactory(IResolver resolver)
        {
            _resolver = resolver;
        }

        public RouterManager CreateManager(Router router, RoutersService owner)
        {
            return new RouterManager(router, owner, _resolver);
        }
    }
}
