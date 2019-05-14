using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.IoC
{
    public class ServiceClass
    {
        public Type ConsumerType { get; private set; }

        public ServiceClass(Type consumer)
        {
            ConsumerType = consumer;
        }

    }
}
