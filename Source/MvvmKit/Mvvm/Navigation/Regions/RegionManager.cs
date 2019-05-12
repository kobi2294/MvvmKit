using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MvvmKit
{
    public class RegionManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public AsyncEvent Navigated { get; } = new AsyncEvent();

        private HashSet<ContentControl> _hosts = new HashSet<ContentControl>();
        private IResolver _resolver;


        public RegionsService OwnerService { get; }

        public Region Region { get; }


        public IEnumerable<ContentControl> Hosts
        {
            get
            {
                return _hosts.ToArray();
            }
        }


        public NavigationEntry CurrentNavigation { get; private set; }

        public ComponentBase CurrentViewModel { get; set; }

        public DataTemplateSelector ViewSelector { get; }


        public RegionManager(Region region, RegionsService owner, IResolver resolver)
        {
            CurrentNavigation = NavigationEntry.Empty;
            _resolver = resolver;
            OwnerService = owner;
            Region = region;

            ViewSelector = new ViewTemplateSelector { ViewResolver = _resolver.Resolve<IViewResolver>() };
        }

        private void _setCurrentViewModel(ComponentBase vm)
        {
            CurrentViewModel = vm;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentViewModel)));
        }

        private Task _invokeHostsHook(Func<RegionHostBehavior, Func<RegionManager, ContentControl, Task>> func)
        {
            var tasks = _hosts.Select(h => (host: h, behavior: RegionHost.GetBehavior(h)))
                .Where(pair => pair.behavior != null)
                .Select(pair => func(pair.behavior).Invoke(this, pair.host))
                .ToArray();

            return Task.WhenAll(tasks);
        }

        private Task _invokeHostsBeforeNavigation()
        {
            return _invokeHostsHook(behavior => behavior.BeforeNavigation);
        }

        private Task _invokeHostsAfterNavigation()
        {
            return _invokeHostsHook(behavior => behavior.AfterNavigation);
        }

        public async Task<T> NavigateTo<T>(object param = null)
            where T : ComponentBase
        {
            var entry = NavigationEntry.Create<T>(param);
            var vm = await _navigateTo(entry, param);
            return vm as T;
        }
        public async Task<ComponentBase> NavigateTo(Type t, object param = null)
        {
            var entry = NavigationEntry.Create(t, param);
            return await _navigateTo(entry, param);
        }

        public async Task<ComponentBase> NavigateTo(Func<Task<ComponentBase>> vmFactory, object param = null)
        {
            var entry = NavigationEntry.CreateWithFactory(vmFactory, param);
            return await _navigateTo(entry, param);
        }

        public async Task<T> Dialog<VMT, T>(object param = null)
            where VMT : DialogBase<T>
        {
            var vm = await NavigateTo<VMT>(param);
            var res = await vm.Task;

            if (vm == CurrentViewModel)
                await Clear();
            return res;
        }

        public async Task Clear()
        {
            var entry = NavigationEntry.Empty;
            await _navigateTo(entry, null);
        }

        public bool IsHost(ContentControl host)
        {
            return _hosts.Contains(host);
        }

        public bool HasHosts
        {
            get
            {
                return _hosts.Any();
            }
        }

        private async Task<ComponentBase> _navigateTo(NavigationEntry entry, object param)
        {
            if (entry == CurrentNavigation) return CurrentViewModel;
            CurrentNavigation = entry;

            ComponentBase vm = null;
            if (entry != NavigationEntry.Empty)
            {
                if (entry.Factory == null)
                {
                    vm = _resolver.Resolve(entry.ViewModelType) as ComponentBase;
                }
                else
                {
                    vm = await entry.Factory();
                }
            }

            // we test the local variable against the class field, to stop cases where a new navigation command is in effect, 
            // and in these cases we abort the opreation
            var currentNavigation = CurrentNavigation;

            await Task.WhenAll(
                Region.InvokeBehaviorsBeforeNavigation(),
                _invokeHostsBeforeNavigation());

            if ((CurrentViewModel != null) && (CurrentViewModel.IsActive))
                await CurrentViewModel.Deactivate();

            if (vm != null)
                await vm.Initialize(param);

            // we do not start activation of view model, if it was already navigated from
            if (CurrentNavigation != currentNavigation) return vm;

            _setCurrentViewModel(vm);
            await Navigated.Invoke();


            if (vm != null)
                await vm.Activate();

            await Task.WhenAll(
                Region.InvokeBehaviorsAfterNavigation(),
                _invokeHostsAfterNavigation());

            return vm;
        }

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
                var b1 = new Binding(nameof(CurrentViewModel));
                b1.Source = this;
                cc.SetBinding(props.contentProperty, b1);

                // create binding to data template selector
                var b2 = new Binding(nameof(ViewSelector));
                b2.Source = this;
                cc.SetBinding(props.selectorProperty, b2);
            }
        }

        internal void AddHost(ContentControl cc)
        {
            // first let's make sure we acutally get something as parameter
            if (cc == null) return;

            // make sure the host really wants to be yours :-)
            if (RegionHost.GetRegion(cc) != Region)
                RegionHost.SetRegion(cc, Region);

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
    }
}
