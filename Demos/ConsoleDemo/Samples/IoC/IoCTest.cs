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
            container.AddFactoryFor(type => LogManager.GetLogger(type));

            container.AddFactoryFor(type => new ServiceClass(type));

            var consumer = container.Resolve<ConsumerClass>();
            Console.WriteLine("My service consumer type: " + consumer.Service.ConsumerType);
            Console.WriteLine("My logger name: " + consumer.Logger.Logger.Name);
        }
    }
}
