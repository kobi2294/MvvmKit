using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Unity;

namespace MvvmKit
{
    public class RegionService : UiServiceBase
    {
        #region Public Navigation Interface

        private Task<ComponentBase> _navigateTo(Type t, object param = null)
        {
            var entry = RegionEntry.Create(t, param);
            return _navigateTo(entry);
        }

        private async Task<TVM> _navigateTo<TVM>(object param = null)
            where TVM: ComponentBase
        {
            var entry = RegionEntry.Create<TVM>(param);
            var vm = await _navigateTo(entry);
            return vm as TVM;
        }


        public Task<ComponentBase> NavigateTo(Type t, object param = null)
        {
            return Run(() =>
            {
                return _navigateTo(t, param);
            });
        }

        public Task<TVM> NavigateTo<TVM>(object param = null)
            where TVM: ComponentBase
        {
            return Run(() =>
            {
                return _navigateTo<TVM>(param);
            });
        }

        private async Task _clear()
        {
            await _navigateTo(RegionEntry.Empty);
        }

        public Task Clear()
        {
            return Run(async () => await _clear());
        }

        public Task<T> RunDialog<TVM, T>(object param = null)
            where TVM: DialogBase<T> 
        {
            return Run(async () =>
            {
                var vm = await _navigateTo<TVM>(param);
                var res = await vm.Task;

                if (vm == _currentVm)
                    await _clear();
                return res;
            });
        }

        public Task RunDialog<TVM>(object param = null)
            where TVM : DialogBase
        {
            return Run(async () =>
            {
                var vm = await _navigateTo<TVM>(param);
                await vm.Task;

                if (vm == _currentVm)
                    await _clear();
            });
        }


        private async Task<ComponentBase> _routeTo(Route route, object param = null)
        {
            if (!_region.Routes.Contains(route))
                throw new InvalidOperationException("Route does not belong to the current region");

            var vmType = route.ViewModelType;
            if (route.ParameterMode != RouteParameterMode.Variant)
                param = route.Parameter;

            var res = await _navigateTo(vmType, param);
            return res;
        }

        public Task<ComponentBase> RouteTo(Route route, object param = null)
        {
            return Run(() =>
            {
                return _routeTo(route, param);
            });
        }

        public Task<ComponentBase> RouteTo(object key, object parameter = null)
        {
            return Run(() =>
            {
                var route = _region.Routes.FirstOrDefault(r => Equals(r.Key, key));
                if (route == null)
                    throw new InvalidOperationException("Region does not contain route with the provided key: " + key);
                return _routeTo(route, parameter);
            });
        }

        #endregion

        #region Internal Host Management

        private (DependencyProperty contentProperty, DependencyProperty selectorProperty) _getContentProperties(ContentControl cc, string name)
        {
            var contentProperty = cc.GetDependencyPropertyByName(name);
            var selectorProperty = cc.GetDependencyPropertyByName(name + "TemplateSelector");

            return (contentProperty, selectorProperty);
        }

        internal void ChangeHostContentProperty(ContentControl cc, string oldVal, string newVal)
        {
            if (!string.IsNullOrWhiteSpace(oldVal))
            {
                var props = _getContentProperties(cc, oldVal);
                BindingOperations.ClearBinding(cc, props.contentProperty);
                BindingOperations.ClearBinding(cc, props.selectorProperty);
            }

            if (!string.IsNullOrWhiteSpace(newVal))
            {
                var props = _getContentProperties(cc, newVal);

                // create binding to our CurrentViewModel property
                var b1 = new Binding(nameof(_hostBindable.ViewModel));
                b1.Source = _hostBindable;
                cc.SetBinding(props.contentProperty, b1);

                // create binding to data template selector
                var b2 = new Binding(nameof(_hostBindable.ViewSelector));
                b2.Source = _hostBindable;
                cc.SetBinding(props.selectorProperty, b2);
            }
        }

        internal void AddHost(ContentControl cc)
        {
            // first let's make sure we acutally get something as parameter
            if (cc == null) return;

            // make sure the host really wants to be yours :-)
            if (RegionHost.GetRegion(cc) != _region)
                RegionHost.SetRegion(cc, _region);

            // now make sure it is not already in the list of hosts
            if (_hosts.Contains(cc)) return;

            // so we actually need to set this as a new host
            _hosts.Add(cc);

            // remember to remove it when it is unloaded
            cc.Unloaded += (s, e) =>
            {
                RemoveHost(cc);
            };

            // for window objects, unload is not enough, so we also listen to closed
            if (cc is Window w)
            {
                w.Closed += (s, e) =>
                {
                    RemoveHost(w);
                };
            }

            ChangeHostContentProperty(cc, null, RegionHost.GetContentProperty(cc));
        }

        internal void RemoveHost(ContentControl cc)
        {
            // make sure it is not null
            if (cc == null) return;

            // make suer it is out host to begin with...
            if (!_hosts.Contains(cc)) return;

            if (RegionHost.GetRegion(cc) != null)
                RegionHost.SetRegion(cc, null);


            _hosts.Remove(cc);

            // clear the binding
            ChangeHostContentProperty(cc, RegionHost.GetContentProperty(cc), null);
        }

        #endregion

        #region Internal Routing management

        private async Task _invalidateCurrentRouteEntry()
        {
            var route = _currentRegionEntry.FindMatchingRoute(_region);
            var routeEntry = RouteEntry.Empty;
            if (route != null)
            {
                routeEntry = RouteEntry.Create(route, _currentRegionEntry.Parameter);
            }

            if (routeEntry != _currentRouteEntry)
            {
                _currentRouteEntry = routeEntry;
                await Routed.Invoke(routeEntry);
            }
        }

        #endregion

        #region Events

        public AsyncEvent<RegionEntry> Navigated { get; } = new AsyncEvent<RegionEntry>();
        public AsyncEvent<RouteEntry> Routed { get; } = new AsyncEvent<RouteEntry>();

        #endregion

        // mutating state
        private HashSet<ContentControl> _hosts;
        private RegionEntry _currentRegionEntry;
        private RouteEntry _currentRouteEntry;
        private RegionServiceBindable _hostBindable;
        private NavigationService _owner;

        public IEnumerable<ContentControl> Hosts => _hosts.ToArray();
        public RegionEntry CurrentRegionEntry => _currentRegionEntry;

        public RouteEntry CurrentRouteEntry => _currentRouteEntry;

        public NavigationService Owner => _owner;

        public Region Region => _region;

        public ComponentBase CurrentViewModel => _currentVm;


        private ComponentBase _currentVm
        {
            get => _hostBindable.ViewModel;
            set => _hostBindable.ViewModel = value;
        }

        // imutable constants
        private Region _region;

        // injectables
        private IResolver _resolver;

        private bool _isDisposed;


        public RegionService(Region region, NavigationService owner, IResolver resolver)
        {
            _hosts = new HashSet<ContentControl>();
            _currentRegionEntry = RegionEntry.Empty;
            _currentRouteEntry = RouteEntry.Empty;
            _owner = owner;
            _region = region;
            _hostBindable = new RegionServiceBindable
            {
                ViewModel = null,
                ViewSelector = null
            };
            _resolver = resolver;
            _hostBindable.ViewSelector = new ViewTemplateSelector { ViewResolver = _resolver.Resolve<IViewResolver>() };
        }

        internal Task OnUnregistering()
        {
            return Run(async () =>
            {
                // clear the region before unregistering
                await _navigateTo(RegionEntry.Empty);
                _isDisposed = true;
            });

        }

        private Task _invokeOnAllBehaviors(Func<RegionBehavior, Func<RegionService, Task>> func)
        {
            var tasks = _region.Behaviors.Select(behavior => func(behavior).Invoke(this));
            return Task.WhenAll(tasks);
        }

        private Task _invokeOnAllHostBehaviors(Func<RegionHostBehavior, Func<RegionService, ContentControl, Task>> func)
        {
            var tasks = _hosts.Select(h => (host: h, behavior: RegionHost.GetBehavior(h)))
                    .Where(pair => pair.behavior != null)
                    .Select(pair => func(pair.behavior).Invoke(this, pair.host))
                    .ToArray();

            return Task.WhenAll(tasks);
        }

        private async Task<ComponentBase> _navigateTo(RegionEntry entry)
        {
            var res = await _doActualNavigation(entry);

            // invalidate the current routing calculation, and raise the Routed event if needed
            await _invalidateCurrentRouteEntry();

            return res;
        }

        // this is the main logic of the service - the actual navigation.
        // It performs deactivation of current VM and activation of new vms
        // It also calls the behaviors, and keeps track of navigation entries.
        // lastly, it invokes the invalidation of the current route
        private async Task<ComponentBase> _doActualNavigation(RegionEntry entry)
        {
            if (_isDisposed)
                throw new InvalidOperationException($"Cannot navigate, RegionService {_region} Was unregistered and disposed ");

            // Check if navigation is required at all
            if (entry == _currentRegionEntry) return _currentVm;

            _currentRegionEntry = entry;

            // call behaviors before navigation both on region and on hosts
            await Task.WhenAll(
                _invokeOnAllBehaviors(b => b.BeforeNavigation),
                _invokeOnAllHostBehaviors(b => b.BeforeNavigation));

            // clear current view model.
            if ((_currentVm != null) && (_currentVm.IsNavigatedTo))
                await _currentVm.Clear();

            // after every await - verify we still need to proceed - since there may be concurrent navigation
            if (_currentRegionEntry != entry) return null;

            ComponentBase vm = null;
            if (entry != RegionEntry.Empty)
            {
                vm = _resolver.Resolve(entry.ViewModelType) as ComponentBase;
                await vm.Initialize(entry.Parameter);
            }

            // after every await - verify we still need to proceed - since there may be concurrent navigation
            if (_currentRegionEntry != entry) return vm;

            _currentVm = vm;
            await Navigated.Invoke(entry);

            if (vm != null)
                await vm.NavigateTo(_region);

            // call behaviors after navigation
            await Task.WhenAll(
                _invokeOnAllBehaviors(b => b.AfterNavigation), 
                _invokeOnAllHostBehaviors(b => b.AfterNavigation)
                );

            return vm;
        }
    }
}
