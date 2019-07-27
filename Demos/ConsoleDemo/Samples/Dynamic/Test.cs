using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Dynamic
{
    public static class Test
    {
        public static IPoco CreateDynamic()
        {
            dynamic dyn = new Dynamic();
            // fails
            return (IPoco)dyn;
        }


        public static void Run()
        {
            var ip = CreateDynamic();
            ip.Number = 10;
            Console.WriteLine(ip.Number);

        }
    }
}
