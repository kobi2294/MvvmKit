using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class BackgroundServiceBase : ServiceBase
    {
        public BackgroundServiceBase()
            :base(TaskScheduler.Default)
        {}
    }
}
