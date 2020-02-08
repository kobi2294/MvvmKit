using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class Indexify
    {
        public static int[] IndicesIn<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            var targetMap = target
                            .Enumerated()
                            .ToDictionary(pair => pair.item);

            return source
                .Select(item => targetMap[item].index)
                .ToArray();
        }

    }
}
