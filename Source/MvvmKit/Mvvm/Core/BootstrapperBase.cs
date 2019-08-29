using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private HashSet<Type> _services;
        private HashSet<Type> _servicesToInit;
        private AsyncLazyInit _shutDownLazy;

        protected IEnumerable<ServiceBase> AllServices => 
            _services
            .Select(typ => Container.Resolve(typ) as ServiceBase)
            .ToReadOnly();

        public UnityContainer Container { get; private set; }

        public IResolver Resolver => Container.Resolve<IResolver>();

        public NavigationService Navigation => Container.Resolve<NavigationService>();


        public BootstrapperBase()
        {
            _services = new HashSet<Type>();
            _servicesToInit = new HashSet<Type>();
            _shutDownLazy = new AsyncLazyInit(_shutDown);
        }

        private async void _onWindowUnload(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Windows.Count == 0)
            {
                await ShutDown();
            }
        }


        /// <summary>
        /// The method to call at the entry point of your application. It's signature is async void becuase it is meant to be called
        /// from static void Main() or from the void OnStarted override of the Application class.
        /// </summary>
        public async void Run()
        {
            // make sure the application does not close unless buttstrapper shutdown is called
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Listening to all shutdown events of windows, so we can shutdown when the last window is closed.
            EventManager.RegisterClassHandler(typeof(Window), Window.UnloadedEvent, new RoutedEventHandler(_onWindowUnload), true);

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
            // init services
            var servicesInitTasks = _servicesToInit
                .Select(type => Container.Resolve(type) as ServiceBase)
                .Select(service => service.Init())
                .ToList();


            await OnServicesInitializing();

            await InitializeShellOverride();

            await Task.WhenAll(servicesInitTasks);

            await OnServicesInitialized();
        }

        // private method that will be called by the lazy init object, to make sure it is only called once
        private async Task _shutDown()
        {
            await BeforeServicesShutDownOverride();

            var servicesShutdownTasks = _services
                .Select(type => Container.Resolve(type) as ServiceBase)
                .Select(service => service.ShutDown())
                .ToList();

            await servicesShutdownTasks.WhenAll();

            await BeforeShutDownOverride();

            Application.Current.Shutdown();
        }

        public Task ShutDown()
        {
            return _shutDownLazy.Task;
        }

        protected void RegisterService<ServiceType>(bool shouldInit = true)
            where ServiceType : ServiceBase
        {
            RegisterService<ServiceType, ServiceType>(shouldInit);
        }

        protected void RegisterService<InterfaceType, ServiceType>(bool shouldInit = true)
            where ServiceType : ServiceBase, InterfaceType
        {
            _services.Add(typeof(InterfaceType));
            Container.RegisterType<InterfaceType, ServiceType>(new ContainerControlledLifetimeManager());
            if (shouldInit)
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
        /// Use this method to perform cleanups before services start to shut down
        /// </summary>
        /// <returns></returns>
        protected virtual Task BeforeServicesShutDownOverride()
        {
            return Tasks.Empty;
        }

        /// <summary>
        /// Use this method to perform last miniute clean up before shut down
        /// Note that this method will be called only after all services have completed to shut down
        /// </summary>
        /// <returns></returns>
        protected virtual Task BeforeShutDownOverride()
        {
            return Tasks.Empty;
        }

        #endregion


    }
}
