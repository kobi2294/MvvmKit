using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class EnumerableExtensions
    {
        // helper reflection methods. To speed things up a little
        private static MethodInfo EmptyMethod = typeof(Enumerable).GetMethods().Where(m => m.Name == "Empty").FirstOrDefault();
        private static MethodInfo CastMethod = typeof(Enumerable).GetMethod("Cast", new[] { typeof(IEnumerable) });
        private static MethodInfo ToListMethod = typeof(Enumerable).GetMethods().Where(m => m.Name == "ToList").FirstOrDefault();

        public static IEnumerable<T> AsIEnumerable<T>(this T obj)
        {
            yield return obj;
        }

        public static IEnumerable<(int, T)> Enumerated<T>(this IEnumerable<T> source)
        {
            return source.Select((t, i) => (i, t)).ToArray();
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params T[] items)
        {
            return source.Concat(items.AsEnumerable());
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static IList CreateGenericList(this Type itemType)
        {
            var emptyEnum = EmptyMethod
                                .MakeGenericMethod(new[] { itemType })
                                .Invoke(null, new object[] { });

            var list = ToListMethod
                            .MakeGenericMethod(new[] { itemType })
                            .Invoke(null, new object[] { emptyEnum });

            return list as IList;
        }

        public static List<T> CreateGenericList<T>(IEnumerable<T> values)
        {
            var list = ToListMethod
                        .MakeGenericMethod(new[] { typeof(T) })
                        .Invoke(null, new object[] { values });

            return list as List<T>;
        }

        public static IList CreateGenericList(this Type itemType, IEnumerable values)
        {
            // turn values into IEnumerable<itemType>
            var ienum = CastMethod
                        .MakeGenericMethod(itemType)
                        .Invoke(null, new object[] { values });

            // now create a list
            var list = ToListMethod
                        .MakeGenericMethod(itemType)
                        .Invoke(null, new object[] { ienum });

            return list as IList;

        }

        public static IEnumerable<(T, T)> Pairs<T>(this IEnumerable<T> values1, IEnumerable<T> values2)
        {
            var i1 = values1.GetEnumerator();
            var i2 = values2.GetEnumerator();

            while ((i1.MoveNext()) || (i2.MoveNext()))
            {
                yield return (i1.Current, i2.Current);
            }
        }

        public static IEnumerable<T> FirstEquals<T>(this IEnumerable<T> values1, IEnumerable<T> values2)
        {
            var i1 = values1.GetEnumerator();
            var i2 = values2.GetEnumerator();

            while ((i1.MoveNext() && i2.MoveNext() && (i1.Current.Equals(i2.Current))))
            {
                yield return i1.Current;
            }
        }

        public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> recursiveSelector)
        {
            if (source == null)
            {
                yield break;
            }

            foreach (var i in source)
            {
                yield return i;

                var directChildren = recursiveSelector(i);
                var allChildren = SelectRecursive(directChildren, recursiveSelector);

                foreach (var c in allChildren)
                {
                    yield return c;
                }
            }
        }

        public static IEnumerable<T> Leafs<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> children)
        {
            return source.SelectRecursive(children).Where(i => !children(i).Any());
        }

        public static IEnumerable<T> PathToTarget<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> children, Func<T, bool> isTarget)
        {
            if (source == null) yield break;

            foreach (var item in source)
            {
                if (isTarget(item))
                {
                    yield return item;
                    yield break;
                }

                var childs = children(item);
                var path = childs.PathToTarget(children, isTarget);
                if (path.Any() == true) // if found
                {
                    yield return item;
                    foreach (var pathItem in path)
                    {
                        yield return pathItem;
                    }
                    yield break;
                }
            }
        }


        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> comparer)
        {
            return source.Distinct(comparer.ToEqualityComparer());
        }

        public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            int index = 0;
            foreach (T item in source)
            {
                if (predicate(item))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n)
        {
            var toSkip = Math.Max(0, source.Count() - n);
            return source.Skip(toSkip);
        }

        public static IEnumerable<T> PadRight<T>(this IEnumerable<T> source, int length, T padding = default(T))
        {
            int padNum = Math.Max(0, length - source.Count());

            return source.Concat(Enumerable.Repeat(padding, padNum));
        }

        public static IEnumerable<T> PadLeft<T>(this IEnumerable<T> source, int length, T padding = default(T))
        {
            int padNum = Math.Max(0, length - source.Count());

            return Enumerable.Repeat(padding, padNum).Concat(source);
        }

    }
}
