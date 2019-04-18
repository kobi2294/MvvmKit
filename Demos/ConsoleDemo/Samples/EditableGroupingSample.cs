using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples
{
    public static class EditableGroupingSample
    {
        public static void Run()
        {
            var eg = new MvvmKit.EditableGrouping<int, string>(4);
            eg.Reset("Four", "Quattro", "Arba", "Vier");
            eg.Add("yottsu");


            Console.WriteLine($"There are {eg.Count} ways to say 4");
            Console.WriteLine(String.Join(", ", eg));

            eg.RemoveWhere(s => s.Length > 4);
            Console.WriteLine("But these are the short ones:");
            Console.WriteLine(String.Join(", ", eg));

        }
    }
}
