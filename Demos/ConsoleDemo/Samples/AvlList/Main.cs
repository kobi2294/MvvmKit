using MvvmKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.AvlList
{
    public static class Main
    {
        public static void Run()
        {
            var n = 250000;
            List<int> list = null;
            AvlList<int> avlList = null;
            var watch = new Stopwatch();
            var random = new Random();

            _test(() => list = Enumerable.Range(1, n).ToList(), watch, "Creating list");
            _test(() => avlList = Enumerable.Range(1, n).ToAvlList(), watch, "Creating AVL List");

            _test(() =>
            {
                for (int i = 0; i < n; i++)
                {
                    var index = list.IndexOf(i);
                }
            }, watch, $"Finding index of {n} items in list");


            _test(() =>
            {
                for (int i = 0; i < n; i++)
                {
                    var index = avlList.IndexOf(i);
                }
            }, watch, $"Finding index of {n} items in avl");

            _test(() =>
            {
                for (int i = 0; i < n; i++)
                {
                    var r = random.Next(n);
                    list.Remove(r);
                    list.Insert(r, random.Next(n - 1));
                }
            }, watch, "Moving in list");


            _test(() =>
            {
                for (int i = 0; i < n; i++)
                {
                    var r = random.Next(n);
                    avlList.Remove(r);
                    avlList.Insert(r, random.Next(n-1));
                }
            }, watch, "Moving in AvlList");
        }

        private static void _test(Action action, Stopwatch watch, string text)
        {
            Console.WriteLine(text);
            watch.Reset();
            watch.Start();
            action();
            watch.Stop();
            Console.WriteLine(watch.Elapsed);
        }
    }
}
