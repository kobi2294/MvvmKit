using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Diff
{
    public static class Main
    {
        public static void Run()
        {
            var source = new List<string>
            {
                "Hello",
                "World",
                "And",
                "How",
                "Are",
                "You",
                "Today",
                "And",
                "What",
                "Are",
                "You",
                "Doing", 
                "And", 
                "How", 
                "The", 
                "Heck"
            };

            var target = new List<string>
            {
                "Holla",
                "All",
                "The",
                "Nice",
                "People",
                "In",
                "The",
                "World",
                "And",
                "How",
                "The", 
                "Hell",
                "Are",
                "You"
            };
            
            var diff = source.Diff(target, s => s.Substring(0, 2));

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.Join(", ", source));

            source.ApplyDiff(diff,
                onAdd: (at, item) =>
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Add {item} to {at}");
                    Console.WriteLine(string.Join(", ", source));
                    return item;
                }, 

                onRemove: (from, key, item) =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Remove {item} from {from}");
                    Console.WriteLine(string.Join(", ", source));
                }, 

                onMove: (from, to, item) =>
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Move '{item}' from {from} to {to}");
                    Console.WriteLine(string.Join(", ", source));
                }, 

                onModify: (at, oldVal, newVal, item) =>
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Modify at {at} from '{oldVal}' to '{newVal}'");
                    Console.WriteLine(string.Join(", ", source));
                    source[at] = newVal;
                });


            Console.WriteLine("------------------------------------------------------------------------------------------");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.Join(", ", source));
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(string.Join(", ", target));
        }
    }
}
