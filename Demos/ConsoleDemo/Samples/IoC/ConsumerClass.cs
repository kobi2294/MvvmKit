using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace ConsoleDemo.Samples.IoC
{
    public class ConsumerClass1
    {
        public ServiceClass Service { get; private set; }

        public ILog Logger { get; private set; }

        public ConsumerClass1(ILog logger)
        {
            Logger = logger;
        }

        [InjectionMethod]
        public void Inject(ServiceClass service)
        {
            Service = service;
        }
    }

    public class ConsumerClass2
    {
        public ServiceClass Service { get; private set; }

        public ILog Logger { get; private set; }

        public ConsumerClass2(ILog logger)
        {
            Logger = logger;
        }

        [InjectionMethod]
        public void Inject(ServiceClass service)
        {
            Service = service;
        }
    }

}
