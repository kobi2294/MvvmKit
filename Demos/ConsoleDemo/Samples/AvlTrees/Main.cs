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
            var t = new AvlTree<int>();
            var rand = new Random();

            foreach (var item in Enumerable.Range(0, 201))
            {
                t.Add(item);
                if (item % 10 == 0) PrintTree(t);
            }

            t.CheckStructure();

            foreach (var item in Enumerable.Range(0, 41))
            {
                Console.WriteLine($"{item*5}: {t[item*5]}");
            }

            foreach (var item in Enumerable.Range(0, 201))
            {
                var index = rand.Next(0, t.Count);
                t.RemoveAt(index);
                Console.WriteLine($"Removing {index}");

                if (t.Count % 10 == 5) PrintTree(t);
            }

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

            if (node.IsEmpty) Console.ForegroundColor = ConsoleColor.DarkBlue;

            var indent = new string('│', level);
            var val = node.IsEmpty ? " ** " : node.Value.ToString();

            var nodechar = (node.Left == null) && (node.Right == null) ? '└' : '├';

            Console.WriteLine(indent + nodechar + role + ": " + val);

            if (node.Left != null)
                PrintTreeNode(level + 1, "L", node.Left);
            if (node.Right != null)
                PrintTreeNode(level + 1, "R", node.Right);
        }


    }
}
