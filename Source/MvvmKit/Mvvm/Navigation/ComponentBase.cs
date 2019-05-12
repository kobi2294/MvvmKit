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
        public object Parameter { get; private set; }

        #region Bindable Properties

        private bool _IsActive;
        public bool IsActive { get { return _IsActive; } private set { SetProperty(ref _IsActive, value); } }

        #endregion

        public async Task Initialize(object param)
        {
            Parameter = param;
            await OnInitialized();
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
        public void Inject(IResolver resolver)
        {
            Resolver = resolver;
        }

        protected virtual Task OnInitialized()
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
