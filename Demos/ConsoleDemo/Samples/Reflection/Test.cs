using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Reflection
{
    public interface IA
    {
        int A { get; set; }
    }

    public interface IB: IA
    {
        int B { get; set; }
    }

    public static class Test
    {
        public static void Run()
        {
            var props = typeof(IB).GetAllProperties();
            Console.WriteLine(string.Join(", ", props));
        }

    }
}
