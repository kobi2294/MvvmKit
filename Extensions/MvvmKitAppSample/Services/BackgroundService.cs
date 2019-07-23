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
        private readonly ServiceCollectionField<int> _Numbers = (10, 20, 30);
        public ServiceCollectionProperty<int> Numbers { get => (_Numbers, this); }

        private readonly ServiceCollectionField<int> _ReadNumbers = (10, 20, 30);
        public ServiceCollectionPropertyReadonly<int> ReadNumbers { get => (_ReadNumbers, this); }

        private readonly ServiceCollectionField<string> _MyNames = ("a");
        public ServiceCollectionProperty<string> MyNames { get => (_MyNames, this); }

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
            await _MyNames.Add("b");
            await _MyNames.Add("c");
            await _MyNames.Add("d");
            await _MyNames.Add("e");
            await _MyNames.Add("f");
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
            });
        }

        public Task Method2()
        {
            return Run(async () =>
            {
                Debug.WriteLine("I am in a ba service, method 2, thread = " + Thread.CurrentThread.ManagedThreadId);
            });
        }

        public Task Method3()
        {
            return Run(async () =>
            {
                Debug.WriteLine("I am in a ba service, method 3, thread = " + Thread.CurrentThread.ManagedThreadId);
                await Method2();
                Debug.WriteLine("I am in a ba service, method 3, thread = " + Thread.CurrentThread.ManagedThreadId);
                await _uiService.Method2();
                Debug.WriteLine("I am in a ba service, method 3, thread = " + Thread.CurrentThread.ManagedThreadId);
                await Method2();
                Debug.WriteLine("I am in a ba service, method 3, thread = " + Thread.CurrentThread.ManagedThreadId);
            });
        }
    }
}
