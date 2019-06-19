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

        protected NavigationService Navigation { get; private set; }

        public Region Region { get; private set; }

        public object Parameter { get; private set; }

        #region Bindable Properties

        private bool _IsNavigatedTo;
        public bool IsNavigatedTo { get { return _IsNavigatedTo; } set { SetProperty(ref _IsNavigatedTo, value); } }

        private bool _IsInitialized;
        public bool IsInitialized { get { return _IsInitialized; } set { SetProperty(ref _IsInitialized, value); } }

        #endregion

        public async Task Initialize(object param)
        {
            Parameter = param;
            IsInitialized = true;
            await OnInitialized(param);
        }

        internal async Task NavigateTo(Region region)
        {
            Region = region;
            await NavigateTo();
        }

        public async Task NavigateTo()
        {
            IsNavigatedTo = true;
            await OnNavigatedTo();
        }

        public async Task Clear()
        {
            await OnClearing();
            IsNavigatedTo = false;
            Region = null;
        }

        [InjectionMethod]
        public void Inject(IResolver resolver, NavigationService navigation)
        {
            Resolver = resolver;
            Navigation = navigation;
        }

        protected virtual Task OnInitialized(object param)
        {
            return Tasks.Empty;
        }

        protected virtual Task OnNavigatedTo()
        {
            return Tasks.Empty;
        }

        protected virtual Task OnClearing()
        {
            return Tasks.Empty;
        }

    }
}
