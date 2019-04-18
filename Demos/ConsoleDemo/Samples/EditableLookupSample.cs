using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples
{
    public static class EditableLookupSample
    {
        public static void Run()
        {
            // create an enumerable of pairs in the format (5, (a:2, b:3))
            var pairs = from a in Enumerable.Range(1, 10)
                        from b in Enumerable.Range(1, 10)
                        where a <= b
                        select (a + b, (a:a, b:b));

            var el = pairs.ToEditableLookup();
            var keys = el.Keys;

            Console.WriteLine($"There are {keys.Count()} keys: ");  // 19 keys
            Console.WriteLine(string.Join(", ", keys)); // from 2 to 20

            Console.WriteLine($"For example, the key 14 has the following values: ");

            // 4 + 10, 5 + 9, 6 + 8, 7 + 7
            Console.WriteLine(string.Join(", ", el[14].Select(pair => $"{pair.a} + {pair.b}")));

            Console.WriteLine("Now we move all values where a == b");
            el.RemoveWhere(14, pair => pair.a == pair.b);

            // 4 + 10, 5 + 9, 6 + 8
            Console.WriteLine(string.Join(", ", el[14].Select(pair => $"{pair.a} + {pair.b}")));
        }

    }
}
