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

        internal async Task Initialize(Region region, object param)
        {
            Parameter = param;
            Region = region;
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

        internal async Task SaveState(StateSaver state)
        {
            await OnSaveState(state);
        }

        internal async Task Clear()
        {
            await OnClearing();
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

        protected virtual Task OnSaveState(StateSaver state)
        {
            return Tasks.Empty;
        }

        protected virtual Task OnClearing()
        {
            return Tasks.Empty;
        }

    }
}
