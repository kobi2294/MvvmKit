using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace ConsoleDemo.Samples.IoC
{
    public class ConsumerClass
    {
        public ServiceClass Service { get; private set; }

        public ILog Logger { get; private set; }

        [InjectionMethod]
        public void Inject(ServiceClass service, ILog logger)
        {
            Service = service;
            Logger = logger;
        }
    }
}
