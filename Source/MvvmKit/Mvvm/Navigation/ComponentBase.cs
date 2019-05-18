using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKit
{
    public class ComponentBase : BindableBase
    {
        protected IResolver Resolver { get; private set; }

        protected RegionsService Regions { get; private set; }

        protected RoutersService Routers { get; private set; }

        public object Parameter { get; private set; }

        #region Bindable Properties

        private bool _IsActive;
        public bool IsActive { get { return _IsActive; } private set { SetProperty(ref _IsActive, value); } }


        private bool _IsInitialized;
        public bool IsInitialized { get { return _IsInitialized; } set { SetProperty(ref _IsInitialized, value); } }


        #endregion

        public async Task Initialize(object param)
        {
            Parameter = param;
            IsInitialized = true;
            await OnInitialized(param);
        }

        public async Task Activate()
        {
            IsActive = true;
            await OnActivated();
        }

        public async Task Deactivate()
        {
            await OnBeforeDectivated();
            IsActive = false;
        }

        [InjectionMethod]
        public void Inject(IResolver resolver, RegionsService regions, RoutersService routers)
        {
            Resolver = resolver;
            Regions = regions;
            Routers = routers;
        }

        protected virtual Task OnInitialized(object param)
        {
            return Tasks.Empty;
        }

        protected virtual Task OnActivated()
        {
            return Tasks.Empty;
        }

        protected virtual Task OnBeforeDectivated()
        {
            return Tasks.Empty;
        }

    }
}
