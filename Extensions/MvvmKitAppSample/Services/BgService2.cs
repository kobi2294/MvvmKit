using MvvmKit;
using System;
using System.Collections.Generic;
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
