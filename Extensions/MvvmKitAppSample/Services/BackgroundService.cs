using MvvmKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace MvvmKitAppSample.Services
{
    public class BackgroundService: BackgroundServiceBase
    {

        private readonly ServiceField<int> _MyNumber = 55;
        public ServiceProperty<int> MyNumber { get => (_MyNumber, this); }


        private readonly ServiceField<int> _A = 42;
        public ServicePropertyReadonly<int> A { get => (_A, this); }


        private readonly ServiceField<bool> _PropName = true;

        public ServiceProperty<bool> PropName { get => (_PropName, this); }

        public AsyncEvent<DateTime> OnMyBrithday { get; } = new AsyncEvent<DateTime>(DateTime.Now);

        private IUiService _uiService;

        public BackgroundService()
        {
        }

        protected override async Task OnInit()
        {
            await base.OnInit();
        }

        [InjectionMethod]
        public void Inject(IUiService uiService)
        {
            // store dependencies here
            _uiService = uiService;
        }

        public Task Method()
        {
            return Run(async () =>
            {
                Debug.WriteLine("I am in a ba service, thread = " + Thread.CurrentThread.ManagedThreadId);

                await _A.Set(_A.Value + 1);

                await _uiService.Method();
            }, true);
        }
    }
}
