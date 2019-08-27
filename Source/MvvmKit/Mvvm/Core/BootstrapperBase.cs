using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using Unity.Lifetime;

namespace MvvmKit
{
    /// <summary>
    /// A skiny copy of Prism's Unity Bootstrapper that provides a basic bootstrapping sequence
    /// which creates a <see cref="UnityContainer"/>, a shell, and starts registering all the container services
    /// </summary>
    public abstract class BootstrapperBase
    {
        #region Static Access to Bootstrappers

        /*
         * Static members and dependency injection are contradicting terms. Still, Many features of WPF rely on satatic implementation
         * (Attached properties, for example) and they may sometime require static access to the bootstrapper or the container. We therefore
         * provide static API to locate and use the active bootstrapper and container
         */

        private static Dictionary<Application, BootstrapperBase> _bootstrappers;
        static BootstrapperBase()
        {
            _bootstrappers = new Dictionary<Application, BootstrapperBase>();
        }

        /// <summary>
        /// Uses the Application.Current to access a bootstrapper
        /// <b>Use this only from static methods that absolutly must access the current bootstrapper</b>
        /// </summary>
        /// <returns>The Bootstrapper of the current application</returns>
        public static BootstrapperBase Current
        {
            get
            {
                var app = Application.Current;
                if (app == null) return null;

                return _bootstrappers.TryGetValue(app, out var bootstrapper)
                    ? bootstrapper
                    : null;
            }
        }

        /// <summary>
        /// Uses the Bootstrapper.Current to access a bootstrapper, and locates a container inside it. Uses this container to resolve
        /// type T. 
        /// <b>Use this only from static methods that absolutly must access the current container</b>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ResolveStatic<T>()
        {
            var container = Current?.Container;

            return container != null
                ? container.Resolve<T>()
                : default(T);
        }

        #endregion

        private HashSet<Type> _servicesToInit = new HashSet<Type>();
        protected List<ServiceBase> _services;
        public UnityContainer Container { get; private set; }

        public IResolver Resolver => Container.Resolve<IResolver>();

        public NavigationService Navigation => Container.Resolve<NavigationService>();


        /// <summary>
        /// The method to call at the entry point of your application. It's signature is async void becuase it is meant to be called
        /// from static void Main() or from the void OnStarted override of the Application class.
        /// </summary>
        public async void Run()
        {
            _bootstrappers.Add(Application.Current, this);
            Exec.InitUiTaskScheduler();
            Container = CreateContainerOverride();

            if (Container == null)
                throw new InvalidOperationException("Unity container cannot be null");

            Container.RegisterInstance(GetType(), this);
            Container.RegisterType<IResolver, UnityResolver>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IViewResolver, DefaultViewResolver>(new ContainerControlledLifetimeManager());
            RegisterService<NavigationService>();

            await ConfigureContainerOverride();
            _services = _servicesToInit.Select(type => Container.Resolve(type) as ServiceBase).ToList();
            // init services
            var servicesInitTasks = _services.Select(service => service.Init()).ToList();

            await OnServicesInitializing();

            await InitializeShellOverride();

            await Task.WhenAll(servicesInitTasks);

            await OnServicesInitialized();
        }

        public async Task ShutDown()
        {
            await BeforeShutDownOverride();
            Application.Current.Shutdown();
        }

        protected void RegisterService<ServiceType>(bool initService = true)
            where ServiceType : ServiceBase
        {
            Container.RegisterType<ServiceType>(new ContainerControlledLifetimeManager());

            if (initService)
            {
                _servicesToInit.Add(typeof(ServiceType));
            }
        }

        protected void RegisterService<InterfaceType, ServiceType>(bool initService = true)
            where ServiceType : ServiceBase, InterfaceType
        {
            Container.RegisterType<InterfaceType, ServiceType>(new ContainerControlledLifetimeManager());
            if (initService)
            {
                _servicesToInit.Add(typeof(InterfaceType));
            }
        }


        #region Overridables

        protected virtual UnityContainer CreateContainerOverride()
        {
            return new UnityContainer();
        }

        /// <summary>
        /// Use this function to configure container class resolution rules
        /// </summary>
        protected virtual Task ConfigureContainerOverride()
        {
            return Tasks.Empty;
        }

        protected virtual Task OnServicesInitializing()
        {
            return Tasks.Empty;
        }

        protected virtual Task OnServicesInitialized()
        {
            return Tasks.Empty;
        }

        /// <summary>
        /// Use this method to start the initialization sequence of the shell
        /// </summary>
        protected virtual Task InitializeShellOverride()
        {
            return Tasks.Empty;
        }

        /// <summary>
        /// Use this method to perform last miniute clean up before shut down
        /// </summary>
        /// <returns></returns>
        protected virtual Task BeforeShutDownOverride()
        {
            return Tasks.Empty;
        }

        #endregion


    }
}
