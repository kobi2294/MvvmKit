using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class RouterBuilder
    {
        private List<Route> _routes = new List<Route>();
        private Region _region;
        private Route _latestRoute = null;

        internal RouterBuilder(Region region)
        {
            _region = region;
        }

        public RouterBuilder AddRouteTo<TVM>(object key, string name = null)
            where TVM : ComponentBase
        {
            if (string.IsNullOrWhiteSpace(name))
                name = key.ToString();

            var route = new Route
            {
                Key = key,
                Name = name,
                ViewModelType = typeof(TVM)
            };

            _routes.Add(route);
            _latestRoute = route;
            return WithoutParam();
        }

        public RouterBuilder WithoutParam()
        {
            _latestRoute.RouteType = RouteType.Parameterless;
            _latestRoute.ParameterType = null;
            _latestRoute.Parameter = null;
            return this;
        }

        public RouterBuilder WithParam<T>(T param)
        {
            _latestRoute.RouteType = RouteType.FixedPerameter;
            _latestRoute.Parameter = param;
            _latestRoute.ParameterType = typeof(T);
            return this;
        }

        public RouterBuilder WithParam<T>()
        {
            _latestRoute.RouteType = RouteType.VariantParameter;
            _latestRoute.ParameterType = typeof(T);
            _latestRoute.Parameter = null;
            return this;
        }

        public Router Build()
        {
            var res = new Router(_routes, _region);
            return res;
        }
    }

}
