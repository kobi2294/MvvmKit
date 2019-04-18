using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples
{
    public static class LazyDictionaryDemo
    {
        public static void Run()
        {
            var ld = new LazyDictionary<string, ConsoleColor>(s => s.ParseEnum<ConsoleColor>());
            Console.WriteLine($"There are {ld.Count} Items"); // There are 0 items

            var cred = ld["Red"];

            Console.WriteLine($"cred: {cred}");    // cred: Red

            ld["Red"] = ConsoleColor.DarkRed;
            cred = ld["White"];

            Console.WriteLine($"There are {ld.Count} Items"); // There are 2 items

            // Key: Red, Value: DarkRed
            // Key: White, Value: White
            foreach (var pair in ld)    
            {
                Console.WriteLine($"Key: {pair.Key}, Value: {pair.Value}");
            }

        }
    }
}
