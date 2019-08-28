using MvvmKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKitAppSample.Services
{
    public class BgService2 : BackgroundServiceBase, IBgService2
    {
        public AsyncEvent<string> OnString { get; } = new AsyncEvent<string>("First Value");

        protected override async Task OnInit()
        {
            await base.OnInit();

            // init logic here
        }

        protected async override Task OnShutDown()
        {
            await base.OnShutDown();

            Debug.WriteLine("BG Service 2 started shutting down");
            await Task.Delay(1000);
            Debug.WriteLine("BG Service 2 Completed shutting down");
        }

        public static int counter = 0;
        public int id;

        public BgService2()
        {
            id = ++counter;
        }

        [InjectionMethod]
        public void Inject()
        {
            // store dependencies here
        }

        public Task Method()
        {
            return Run(() =>
            {
                // your logic here
            });
        }
    }
}
