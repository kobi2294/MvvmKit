using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class ImmutableCollectionsExtensions
    {
        public static ImmutableList<T> Move<T>(this ImmutableList<T> source,  int fromIndex, int toIndex)
        {
            var item = source[fromIndex];
            return source
                .RemoveAt(fromIndex)
                .Insert(toIndex, item);

        }
    }
}
