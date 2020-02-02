using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmKit;

namespace ConsoleDemo.Samples.Lis
{
    public static class LisTest
    {
        public static void Run()
        {
            int[] arr = { 28, 2, 20, 5, 3, 7, 25, 11, 0, 27, 8, 21, 15, 1, 4, 6, 24, 12, 18, 9, 10, 13, 30, 19, 16, 14, 17, 29, 23, 26, 22 };

            var lis = MvvmKit.Lis.Calculate(arr);
            var text = string.Join(" ", lis);

            Console.WriteLine(text);
            Console.WriteLine("Size: " + lis.Length);
        }
    }
}
