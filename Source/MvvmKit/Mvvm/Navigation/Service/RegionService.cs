﻿using System;
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
            where TVM : ComponentBase
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

        public Task<ComponentBase> NavigateBack()
        {
            return Run(_navigateBack);
        }

        public Task<TVM> NavigateTo<TVM>(object param = null)
            where TVM : ComponentBase
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
            where TVM : DialogBase<T>
        {
            return Run(async () =>
            {
                var vm = await _navigateTo<TVM>(param);
                var res = await vm.Task;

                if (vm == _CurrentViewModel.Value)
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

                if (vm == _CurrentViewModel.Value)
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

        private async Task _runOnDestroy(StateStore state)
        {
            await state.Read(async reader =>
            {
                var func = reader.GetAnnotation<Func<StateReader, Task>>("destroy");
                await func(reader);
            });
        }

        private async Task _destroyEntryStateWhere(Func<RegionEntry, bool> entryPicker)
        {
            var entries = _savedStates.Keys.Where(entryPicker).ToArray();
            var states = entries.Select(e => _savedStates.GetAndRemove(e)).ToArray();

            var destroyTasks = states.Select(s => _runOnDestroy(s));
            await Task.WhenAll(destroyTasks);
        }

        public Task DestroyEntryStateWhere(Func<RegionEntry, bool> entryPicker)
        {
            return Run(() => _destroyEntryStateWhere(entryPicker));
        }

        private async Task _moveEntryStateWhere(Func<RegionEntry, bool> entryPicker, RegionService target)
        {
            var entries = _savedStates.Keys.Where(entryPicker).ToArray();
            var states = entries.ToDictionary(entry => entry, entry => _savedStates.GetAndRemove(entry));
            await target._importEntryStates(states);
        }

        private Task _importEntryStates(Dictionary<RegionEntry, StateStore> states)
        {
            return Run(() =>
            {
                foreach (var pair in states)
                {
                    _savedStates[pair.Key] = pair.Value;
                }
            });
        }

        public Task MoveEntryStateWhere(Func<RegionEntry, bool> entryPicker, RegionService target)
        {
            return Run(() => _moveEntryStateWhere(entryPicker, target));
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
            cc.Unloaded += _onHostUnloaded;

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

        private void _onHostUnloaded(object sender, RoutedEventArgs e)
        {
            var cc = sender as ContentControl;
            if (cc != null)
            {
                // catering for the chance that the content control may be "reloaded" we are prepaired to return it to the hosts collection
                cc.Unloaded -= _onHostUnloaded;
                cc.Loaded += _onHostLoaded;
                RemoveHost(cc);
            }
        }

        private void _onHostLoaded(object sender, RoutedEventArgs e)
        {
            var cc = sender as ContentControl;
            if (cc != null)
            {
                cc.Loaded -= _onHostLoaded;
                AddHost(cc);
                cc.Unloaded += _onHostUnloaded;
            }
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
            var route = _CurrentRegionEntry.Value.FindMatchingRoute(_region);
            var routeEntry = RouteEntry.Empty;
            if (route != null)
            {
                routeEntry = RouteEntry.Create(route, _CurrentRegionEntry.Value.Parameter);
            }

            if (routeEntry != _CurrentRouteEntry.Value)
            {
                await _CurrentRouteEntry.Set(routeEntry);
            }
        }

        #endregion

        // mutating state

        private readonly ServiceCollectionField<RegionEntry> _History = new ServiceCollectionField<RegionEntry>();
        public ServiceCollectionPropertyReadonly<RegionEntry> History { get => (_History, this); }

        private readonly ServiceField<RegionEntry> _CurrentRegionEntry = RegionEntry.Empty;
        public ServicePropertyReadonly<RegionEntry> CurrentRegionEntry { get => (_CurrentRegionEntry, this); }


        private readonly ServiceField<RouteEntry> _CurrentRouteEntry = RouteEntry.Empty;
        public ServicePropertyReadonly<RouteEntry> CurrentRouteEntry { get => (_CurrentRouteEntry, this); }

        private readonly ServiceField<ComponentBase> _CurrentViewModel = new ServiceField<ComponentBase>(null);
        public ServicePropertyReadonly<ComponentBase> CurrentViewModel { get => (_CurrentViewModel, this); }


        private HashSet<ContentControl> _hosts;
        private RegionServiceBindable _hostBindable;
        private NavigationService _owner;
        private Dictionary<RegionEntry, StateStore> _savedStates;

        public IEnumerable<ContentControl> Hosts => _hosts.ToArray();

        public NavigationService Owner => _owner;

        public Region Region => _region;

        // imutable constants
        private Region _region;

        // injectables
        private IResolver _resolver;

        private bool _isDisposed;


        public RegionService(Region region, NavigationService owner, IResolver resolver)
        {
            _hosts = new HashSet<ContentControl>();
            _savedStates = new Dictionary<RegionEntry, StateStore>();
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

                // destroy all saved states
                await _destroyEntryStateWhere(r => true);

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

        private async Task<ComponentBase> _navigateBack()
        {
            var entry = await _History.Pop();
            var res = await _doActualNavigation(entry, true);

            // invalidate the current routing calculation, and raise the Routed event if needed
            await _invalidateCurrentRouteEntry();

            return res;
        }

        // this is the main logic of the service - the actual navigation.
        // It performs deactivation of current VM and activation of new vms
        // It also calls the behaviors, and keeps track of navigation entries.
        private async Task<ComponentBase> _doActualNavigation(RegionEntry entry, bool isBack = false)
        {
            if (_isDisposed)
                throw new InvalidOperationException($"Cannot navigate, RegionService {_region} Was unregistered and disposed ");

            // Check if navigation is required at all
            if (entry == _CurrentRegionEntry.Value) return _CurrentViewModel.Value;

            var oldEntry = _CurrentRegionEntry.Value;
            await _CurrentRegionEntry.Set(entry);

            if (!isBack)
                await _History.Add(oldEntry);

            // call behaviors before navigation both on region and on hosts
            await Task.WhenAll(
                _invokeOnAllBehaviors(b => b.BeforeNavigation),
                _invokeOnAllHostBehaviors(b => b.BeforeNavigation));

            // clear current view model.
            if ((_CurrentViewModel.Value != null) && (_CurrentViewModel.Value.RegionService == this))
            {
                var state = await StateStore.Write(_CurrentViewModel.Value, async writer =>
                {
                    writer.WriteAnnotation<Func<StateReader, Task>>("destroy", _CurrentViewModel.Value.OnDestroyState);
                    await _CurrentViewModel.Value.SaveState(writer);
                });
                _savedStates.Add(oldEntry, state);

                await _CurrentViewModel.Value.Clear();
            }

            // after every await - verify we still need to proceed - since there may be concurrent navigation
            if (_CurrentRegionEntry.Value != entry) return null;

            ComponentBase vm = null;
            if (entry != RegionEntry.Empty)
            {
                vm = _resolver.Resolve(entry.ViewModelType) as ComponentBase;

                await vm.Initialize(this, entry.Parameter);

                var isRestore = (_savedStates.ContainsKey(entry));
                if (isRestore)
                {
                    var state = _savedStates.GetAndRemove(entry);
                    using (var restorer = new StateRestorer(vm, state))
                    {
                        restorer.RunSetters();
                        await vm.RestoreState(restorer);
                        state.Clear();
                    }
                }
                else
                {
                    await vm.NewState();
                }
            }

            // after every await - verify we still need to proceed - since there may be concurrent navigation
            if (_CurrentRegionEntry.Value != entry) return vm;

            _hostBindable.ViewModel = vm;
            await _CurrentViewModel.Set(vm);

            if (vm != null)
                await vm.NavigateTo();

            // call behaviors after navigation
            await Task.WhenAll(
                _invokeOnAllBehaviors(b => b.AfterNavigation),
                _invokeOnAllHostBehaviors(b => b.AfterNavigation)
                );

            return vm;
        }
    }
}
