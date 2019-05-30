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
    public class BgService1 : BackgroundServiceBase, IBgService1
    {
        private IBgService2 _svc;

        protected override async Task OnInit()
        {
            await base.OnInit();

            // init logic here
            await _svc.OnString.Subscribe(this, OnString);
        }

        private async Task OnString(string arg)
        {
            await Task.Delay(1000);
            Debug.WriteLine("Hey Baby! " + arg);
        }

        [InjectionMethod]
        public void Inject(IBgService2 svc)
        {
            // store dependencies here
            _svc = svc;
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
