using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace $rootnamespace$
{
    public class $safeitemname$: UiServiceBase
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

        public Task Method()
        {
            return Run(() =>
            {
                // your logic here
            });
        }
    }
}
