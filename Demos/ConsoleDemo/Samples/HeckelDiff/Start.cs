using MvvmKit.HeckelDiff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MvvmKit.HeckelDiff.Operations;

namespace ConsoleDemo.Samples.HeckelDiff
{
    public static class Start
    {

        public static void Run()
        {
            var o = new List<int> { 1, 2, 3, 7, 5, 9 };
            var n = new List<int> { 7, 2, 5, 9, 3, 1}; 

            var ops = Heckel.Diff(o, n);

            foreach (var op in ops)
            {
                Console.WriteLine(op);
            }

            //Operation op = Insert(12);            
            //Console.WriteLine(op);

            //op = Move(10, 20);
            //Console.WriteLine(op);

            //Counter c = Counter.Zero;
            //Console.WriteLine(c);
            //c = c.Increment();
            //Console.WriteLine(c);
            //c = c.Increment();
            //Console.WriteLine(c);
            //c = c.Increment();
            //Console.WriteLine(c);
        }
    }
}
