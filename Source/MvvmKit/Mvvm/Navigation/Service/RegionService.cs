using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MvvmKit
{
    public class RegionService : UiServiceBase
    {
        #region Public Navigation Interface

        public Task<ComponentBase> NavigateTo(Type t, object param = null)
        {
            throw new NotImplementedException();
        }

        public Task<ComponentBase> NavigateTo<TVM>(object param = null)
            where TVM: ComponentBase
        {
            throw new NotImplementedException();
        }

        public Task<T> RunDialog<TVM, T>(object param = null)
            where TVM: DialogBase<T> 
        {
            throw new NotImplementedException();
        }

        public Task<ComponentBase> RouteTo(Route route, object parameter = null)
        {
            throw new NotImplementedException();
        }

        public Task<ComponentBase> RouteTo(object key, object parameter = null)
        {
            throw new NotImplementedException();
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Internal Host Management

        internal void ChangeHostContentProperty(ContentControl cc, string oldVal, string newVal)
        {
            throw new NotImplementedException();
        }

        internal void AddHost(ContentControl cc)
        {
            throw new NotImplementedException();
        }

        internal void RemoveHost(ContentControl cc)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Events

        public AsyncEvent<RegionEntry> Navigated { get; } = new AsyncEvent<RegionEntry>();
        public AsyncEvent<RouteEntry> Routed { get; } = new AsyncEvent<RouteEntry>();

        #endregion

        // mutating state
        private HashSet<ContentControl> _hosts = new HashSet<ContentControl>();
        private RegionEntry _currentRegionEntry;
        private RouteEntry _currentRouteEntry;
        private ComponentBase _currentVm;

        // imutable constants
        private Region _region;
        private DataTemplateSelector _viewSelector;

        // helper

        // Add a bindable object for the hosts
        // that contains the current view model and the view selector
    }
}
