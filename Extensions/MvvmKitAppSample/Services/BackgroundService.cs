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
        public interface IModel
        {
            IStateList<int> Numbers { get; set; }
            IStateList<int> ReadNumbers { get; set; }
            IStateList<string> MyNames { get; set; }
            int MyNumber { get; set; }
            int A { get; set; }
            bool PropName { get; set; }
            
        }

        private ServiceStore<IModel> _store = new ServiceStore<IModel>(data =>
        {
            data.Numbers.Reset(10, 20, 30);
            data.ReadNumbers.Reset(10, 20, 30);
            data.MyNames.Reset("a");
            data.MyNumber = 55;
            data.A = 42;
            data.PropName = true;
        });

        public IStateCollection<int> Numbers { get; }
        public IStateCollectionReader<int> ReadNumbers { get; }
        public IStateCollection<string> MyNames { get; }
        public IStateProperty<int> MyNumber { get; }
        public IStatePropertyReader<int> A { get; }
        public IStateProperty<bool> PropName { get; }

        public AsyncEvent<DateTime> OnMyBrithday { get; } = new AsyncEvent<DateTime>(DateTime.Now);

        private IUiService _uiService;

        public BackgroundService()
        {
            Numbers = _store.CreateWriter(this, x => x.Numbers);
            ReadNumbers = _store.CreateReader(this, x => x.ReadNumbers);
            MyNames = _store.CreateWriter(this, x => x.MyNames);
            MyNumber = _store.CreateWriter(this, x => x.MyNumber);
            A = _store.CreateReader(this, x => x.A);
            PropName = _store.CreateWriter(this, x => x.PropName);
        }

        protected override async Task OnInit()
        {
            await base.OnInit();
            await _store.Modify(data =>
            {
                data.MyNames.AddRange("b", "c", "d", "e", "f");
            });
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

                await _store.Modify(data => data.A++); 

                await _uiService.Method();
            });
        }

        public Task Method2()
        {
            return Run(() =>
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
