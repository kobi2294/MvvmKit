using ConsoleDemo.Samples.Disposables;
using log4net;
using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;
using Unity.Policy;
using Unity.Resolution;
using Unity.Strategies;

namespace ConsoleDemo
{
    class Program
    {

        static void Main(string[] args)
        {
            var tree = new SortedAvlTree<int>() { 1, 2, 3, 4 };
            var arr = tree.Select(n => n.Item).ToArray();

            foreach (var item in tree)
            {
                tree.Remove(item);
            }

            Samples.AvlTrees.Main.Benchmark();

            Console.ReadLine();
        }
    }
}
