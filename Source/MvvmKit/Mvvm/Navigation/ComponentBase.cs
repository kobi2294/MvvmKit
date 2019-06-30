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

        public RegionService RegionService { get; private set; }

        public object Parameter { get; private set; }

        internal async Task Initialize(RegionService regionService, object param)
        {
            Parameter = param;
            RegionService = regionService;
            await OnInitialized(param);
        }

        internal async Task NewState()
        {
            await OnNewState();
        }

        internal async Task RestoreState(StateRestorer state)
        {
            await OnRestoreState(state);
        }

        internal async Task NavigateTo()
        {
            await OnNavigatedTo();
        }

        internal async Task SaveState(StateWriter state)
        {
            await OnSaveState(state);
        }

        internal async Task Clear()
        {
            await OnClearing();
            RegionService = null;
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

        protected virtual Task OnNewState()
        {
            return Tasks.Empty;
        }

        protected virtual Task OnRestoreState(StateRestorer state)
        {
            return Tasks.Empty;
        }

        protected virtual Task OnNavigatedTo()
        {
            return Tasks.Empty;
        }

        protected virtual Task OnSaveState(StateWriter state)
        {
            return Tasks.Empty;
        }

        protected virtual Task OnClearing()
        {
            return Tasks.Empty;
        }

        protected internal virtual Task OnDestroyState(StateReader state)
        {
            return Tasks.Empty;
        }

    }
}
