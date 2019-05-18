using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class RoutersService
    {
        private RouterManagerFactory _factory;

        private Dictionary<Router, RouterManager> _managers { get; } = new Dictionary<Router, RouterManager>();

        public RoutersService(RouterManagerFactory factory)
        {
            _factory = factory;
        }

        public RouterManager this[Router router]
        {
            get
            {
                return _managers[router];
            }
        }

        public RouterManager this[Route route]
        {
            get
            {
                return _managers[route.Owner];
            }
        }

        public void RegisterRouter(Router router)
        {
            if (router.Owner != null)
                throw new InvalidOperationException("The router already belongs to another RouterService");

            var manager = _factory.CreateManager(router, this);
            router.Owner = this;
            _managers.Add(router, manager);
        }

        /// <summary>
        /// Enumerates all the static properties of type <see cref="Router">Router</see> of this type and registers them.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterStaticRouters(Type type)
        {
            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .ToArray();

            foreach (var pi in properties)
            {
                var router = pi.GetValue(null) as Router;

                RegisterRouter(router);
            }
        }

        public async Task Navigate(Route route, object parameter = null)
        {
            var router = route.Owner;
            var manager = _managers[router];
            await manager.Navigate(route, parameter);
        }

    }
}
