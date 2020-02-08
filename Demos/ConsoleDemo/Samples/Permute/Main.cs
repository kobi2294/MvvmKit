using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Permute
{
    public static class Main
    {
        public static void Print(IEnumerable<char> text, HashSet<char> lis, HashSet<char> moved, char current)
        {
            foreach (var c in text)
            {
                if (c == current) Console.ForegroundColor = ConsoleColor.Green;
                else if (lis.Contains(c)) Console.ForegroundColor = ConsoleColor.Red;
                else if (moved.Contains(c)) Console.ForegroundColor = ConsoleColor.Yellow;
                else Console.ForegroundColor = ConsoleColor.White;

                Console.Write(c);
            }
        }

        public static void Run()
        {
            var rand = new Random();
            var target = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var temp = target.ToList();

            for (int i = 0; i < 1000; i++)
            {
                var f = rand.Next(target.Length);
                var t = rand.Next(target.Length);
                (temp[f], temp[t]) = (temp[t], temp[f]);
            }

            var source = new string(temp.ToArray());
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(source);

            var lis = source.LisBy(target).ToHashSet();
            Print(source.ToArray(), lis, new HashSet<char>(), ' ');
            Console.WriteLine();


            var moves = source.Permute(target);
            var work = source.ToList();
            var moved = new HashSet<char>();

            foreach (var move in moves)
            {
                var i = work[move.from];
                work.RemoveAt(move.from);
                work.Insert(move.to, i);

                Print(work, lis, moved, i);
                moved.Add(i);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($" {i} From {move.from} To {move.to}");
            }

            Console.WriteLine(string.Join("", work));
            Console.WriteLine(string.Join("", target));
        }

    }
}
