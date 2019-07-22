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
    public class UiService : UiServiceBase, IUiService
    {
        protected override async Task OnInit()
        {
            await base.OnInit();

            // init logic here
        }

        [InjectionMethod]
        public void Inject()
        {
            // store dependencies here
        }

        public async Task Method()
        {
            await Run(() =>
            {
                Debug.WriteLine("I am in UI Service, Thread = " + Thread.CurrentThread.ManagedThreadId);
                // your logic here
            });
        }

        public Task Method2()
        {
            return Run(async () =>
            {
                Debug.WriteLine("I am in a ui service, method 2, thread = " + Thread.CurrentThread.ManagedThreadId);
                await Method();
                Debug.WriteLine("I am in a ui service, method 2, thread = " + Thread.CurrentThread.ManagedThreadId);
            });
        }

    }
}
