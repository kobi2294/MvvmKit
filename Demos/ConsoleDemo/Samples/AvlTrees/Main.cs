using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.AvlTrees
{
    public static class Main
    {
        public static void Run()
        {
            var t = new SortedAvlTree<int>();
            var rand = new Random();

            foreach (var item in Enumerable.Range(0, 100))
            {
                t.Add(item);
                if (item % 10 == 9) PrintTree(t);
            }

            foreach (var item in Enumerable.Range(100, 101).Reverse())
            {
                t.Add(item);
                if (item % 10 == 0) PrintTree(t);
            }

            t.CheckStructure();

            foreach (var item in Enumerable.Range(0, 41))
            {
                Console.WriteLine($"{item*5}: {t[item*5].Item}");
            }

            foreach (var item in Enumerable.Range(0, 181))
            {
                var index = rand.Next(0, t.Count);
                t.RemoveAt(index);
                Console.WriteLine($"Removing {index}");

                if (t.Count % 10 == 5) PrintTree(t);
            }

            // 20 items remaining
            Console.WriteLine("-------------------");
            Console.WriteLine("20 items remaining");
            t.CheckStructure();

            PrintTree(t);

            Console.WriteLine("Shuffling");

            foreach (var item in Enumerable.Range(0, 50))
            {
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
                PrintTree(t);
                t.CheckStructure();

                Console.WriteLine($"item[{toIndex}]({toNode.Item}) = {tmp}");
                toNode.Item = tmp;
                idx = t.IndexOf(toNode);
                Console.WriteLine($"Moved to: {idx} ");

                Console.WriteLine($"And original moved to {t.IndexOf(fromNode)}");
                Console.WriteLine($"Tree Height: {t.Height}");
                PrintTree(t);
                t.CheckStructure();
            }

            PrintTree(t);
            t.CheckStructure();
        }

        public static void PrintTree<T>(AvlTree<T> tree)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Count " + tree.Count);
            PrintTreeNode(0, "T", tree.Root);
            Console.WriteLine();


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


    }
}
