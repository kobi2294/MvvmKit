using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Indexify
{
    public static class Main
    {
        public static void Run()
        {
            var source = new string[]
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I"
            };

            var target = new string[]
            {
                "B", "D", "F", "I", "A", "G", "C", "E", "H"
            };

            var res1 = source.IndicesIn(target);
            var res2 = target.IndicesIn(source);

            Console.WriteLine(string.Join(",", source));
            Console.WriteLine(string.Join(",", target));
            Console.WriteLine(string.Join(",", res1));
            Console.WriteLine(string.Join(",", res2));
        }
    }
}
