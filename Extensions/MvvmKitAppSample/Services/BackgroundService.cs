using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKitAppSample.Services
{
    public class BackgroundService: BackgroundServiceBase
    {
        public Task Method()
        {
            return Run(() =>
            {
                // your logic here
            });
        }
    }
}
