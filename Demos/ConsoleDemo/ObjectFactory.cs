using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace ConsoleDemo
{
    public class ObjectFactory
    {
        public Type CallerType { get; private set; }

        public static ObjectFactory Create(Type t)
        {
            return new ObjectFactory() { CallerType = t };
        }
    }
}
