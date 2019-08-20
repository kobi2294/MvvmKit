using MvvmKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.AvlTrees
{
    public static class Main
    {
        public static void Run()
        {
            Console.BufferWidth = 2400;
            var t = new SortedAvlTree<int>();
            var rand = new Random();

            foreach (var item in Enumerable.Range(0, 100))
            {
                var num = (item * 29) % 100;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Adding {num}");
                t.Add(num);
                Console.ForegroundColor = ConsoleColor.White;
                PrintTree(t);

            }

            foreach (var item in Enumerable.Range(100, 701).Reverse())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Adding {item}");
                Console.ForegroundColor = ConsoleColor.White;
                t.Add(item);
                if (item % 20 == 0) PrintTree(t);
            }

            t.CheckStructure();

            foreach (var item in Enumerable.Range(0, 41))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{item*5}: {t[item*5].Item}");
            }

            foreach (var item in Enumerable.Range(0, 781))
            {
                var index = rand.Next(0, t.Count);
                var remnode = t.RemoveAt(index);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Removed [{index}] (item {remnode.Item}), tree count: {t.Count}, tree root: {t.Root.Item}");

                t.CheckStructure();

                Console.ForegroundColor = ConsoleColor.White;
                if (t.Count % 20 == 5) PrintTree(t);

            }

            // 20 items remaining
            Console.WriteLine("-------------------");
            Console.WriteLine("20 items remaining");
            t.CheckStructure();

            PrintTree(t);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Shuffling");

            foreach (var item in Enumerable.Range(0, 50))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                var fromIndex = rand.Next(0, t.Count);
                var toIndex = rand.Next(0, t.Count);

                var fromNode = t[fromIndex];
                var toNode = t[toIndex];
                Console.WriteLine($"Shuffle {item} [{fromIndex}] ({fromNode.Item}) <-----> [{toIndex}] ({toNode.Item})");

                var tmp = fromNode.Item;

                Console.WriteLine($"item[{fromIndex}]({fromNode.Item}) = {toNode.Item}");
                fromNode.Item = toNode.Item;
                var idx = t.IndexOf(fromNode);
                Console.WriteLine($"Moved to: {idx} ");
                Console.ForegroundColor = ConsoleColor.White;
                PrintTree(t);
                t.CheckStructure();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"item[{toIndex}]({toNode.Item}) = {tmp}");
                toNode.Item = tmp;
                idx = t.IndexOf(toNode);
                Console.WriteLine($"Moved to: {idx} ");

                Console.WriteLine($"And original moved to {t.IndexOf(fromNode)}");
                Console.WriteLine($"Tree Height: {t.Height}");
                Console.ForegroundColor = ConsoleColor.White;
                PrintTree(t);
                t.CheckStructure();
            }
        }

        public static void TestOredered()
        {
            var tree = 1.Yield().Concat(5, 15, 20, 40, 60 , 80).ToAvlTree();
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintTree(tree);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("InsertAt(2, 10)");
            tree.InsertAt(2, 10);
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintTree(tree);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("AddFirst(0)");
            tree.AddFirst(0);
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintTree(tree);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("AddLast(100)");
            tree.AddLast(100);
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintTree(tree);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("AddBefore(node(15), 14)");
            var anchor = tree.Find(15);
            tree.AddBefore(anchor, 14);
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintTree(tree);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("AddAfter(node(15), 16)");
            anchor = tree.Find(15);
            var sixteen = tree.AddAfter(anchor, 16);
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintTree(tree);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("AddAfter(node(16), 17)");
            tree.AddAfter(sixteen, 17);
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintTree(tree);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Sort by units digit");
            var desc = tree.Items.ToSortedAvlTree(n => n % 10);
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintTree(desc);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("add 13 to sorted");
            desc.Add(13);
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintTree(desc);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("add 122 to sorted");
            desc.Add(122);
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintTree(desc);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("add 1222 to sorted");
            desc.Add(1222);
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintTree(desc);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Rebuild");
            desc = desc.Items.ToSortedAvlTree(n => n % 10);
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintTree(desc);
        }

        public static void PrintTree<T>(AvlTree<T> tree)
        {
            var console = new StringConsole();
            tree.Root.PrintToConsole(console, topMargin: 0, leftMargin: 0);
            var res = console.ToString();

            Console.BufferWidth = Math.Max(Console.BufferWidth, console.BufferWidth);
            Console.WriteLine(res);

            //Console.ForegroundColor = ConsoleColor.White;
            //Console.WriteLine("Count " + tree.Count);
            //PrintTreeNode(0, "T", tree.Root);
            //Console.WriteLine();


        }

        public static void PrintTreeNode<T>(int level, string role,  AvlTreeNode<T> node)
        {
            switch (role)
            {
                case "T":
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case "L":
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case "R":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                default:
                    break;
            }

            if (node == null) Console.ForegroundColor = ConsoleColor.DarkBlue;

            var indent = new string('│', level);
            var val = node==null ? " ** " : node.Item.ToString();

            var nodechar = (node.Left == null) && (node.Right == null) ? '└' : '├';

            Console.WriteLine(indent + nodechar + role + ": " + val);

            if (node.Left != null)
                PrintTreeNode(level + 1, "L", node.Left);
            if (node.Right != null)
                PrintTreeNode(level + 1, "R", node.Right);
        }

        public static void Benchmark()
        {
            // Benchmark results for 1,000,000 items:
            //          Build Time  | Search Time
            // Tree            5.4  |   1.3 
            // List          191.1  | 446.1

            // Benchmark results for 500,000 items:
            //          Build Time  | Search Time
            // Tree            2.8  |   0.7
            // List           45.0  | 111.3

            // Benchmark results for 200,000 items:
            //          Build Time  | Search Time
            // Tree            1.0  |   0.3
            // List            7.4  |  17.8

            // Benchmark results for 100,000 items:
            //          Build Time  | Search Time
            // Tree            0.5  |   0.1
            // List            1.8  |   4.5

            // Benchmark results for 50,000 items:
            //          Build Time  | Search Time
            // Tree            0.2  |   0.1
            // List            0.5  |   1.5

            // Benchmark results for 20,000 items:
            //          Build Time  | Search Time
            // Tree           0.08  |  0.03
            // List           0.08  |  0.20

            // Benchmark results for 10,000 items:
            //          Build Time  | Search Time
            // Tree           0.04  |  0.01
            // List           0.02  |  0.05

            // Benchmark results for  5,000 items:
            //          Build Time  | Search Time
            // Tree          0.022  |  0.006
            // List          0.004  |  0.012

            // Benchmark results for  1,000 items:
            //          Build Time  | Search Time
            // Tree         0.0088  |  0.0018
            // List         0.0002  |  0.0006

            var amount = 1000000;


            var list = new List<int>();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Adding {amount} items to tree");
            Stopwatch s = new Stopwatch();
            s.Reset();
            s.Start();
            var tree = Enumerable.Range(0, amount).ToAvlTree();
            s.Stop();
            Console.WriteLine($"Tree: Size = {tree.Count}, Height = {tree.Height}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Ellapsed time: {s.Elapsed.TotalSeconds:N4} seconds");

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Adding {amount} items to list");
            s.Reset();
            s.Start();
            list = Enumerable.Range(0, amount).OrderBy(x => x).ToList();
            s.Stop();
            Console.WriteLine($"List: Size = {list.Count}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Ellapsed time: {s.Elapsed.TotalSeconds:N4} seconds");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Searching {amount} items in tree");
            s.Reset();
            s.Start();
            for (int i = 0; i < amount; i++)
            {
                var node = tree.Find(n => n.Item == i
                ? AvlTreeNodeDirection.Root
                : n.Item < i ? AvlTreeNodeDirection.Right : AvlTreeNodeDirection.Left);

                var index = tree.IndexOf(node);
            }
            s.Stop();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Ellapsed time: {s.Elapsed.TotalSeconds:N4} seconds");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Searching {amount} items in list");
            s.Reset();
            s.Start();
            for (int i = 0; i < amount; i++)
            {
                var index = list.IndexOf(i);
            }
            s.Stop();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Ellapsed time: {s.Elapsed.TotalSeconds:N4} seconds");
        }


    }
}
