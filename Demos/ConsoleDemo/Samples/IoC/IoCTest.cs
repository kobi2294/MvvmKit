using log4net;
using log4net.Repository.Hierarchy;
using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace ConsoleDemo.Samples.IoC
{
    public static class IoCTest
    {
        public static void Run()
        {
            var container = new UnityContainer();

            container.RegisterType<string, string>();

            container.AddFactoryFor(type => LogManager.GetLogger(type ?? typeof(IoCTest)));
            container.AddFactoryFor(type => new ServiceClass(type ?? typeof(IoCTest)));

            var log = container.Resolve<ILog>();

            var consumer1 = container.Resolve<ConsumerClass1>();
            var consumer2 = container.Resolve<ConsumerClass2>();

            Console.WriteLine("1:");
            Console.WriteLine("My logger consumer type: " + consumer1.Service.ConsumerType.Name);
            Console.WriteLine("My service consumer type: " + consumer1.Logger.Logger.Name);
            Console.WriteLine("2:");
            Console.WriteLine("My logger consumer type: " + consumer2.Service.ConsumerType.Name);
            Console.WriteLine("My service consumer type: " + consumer2.Logger.Logger.Name);
        }
    }
}
