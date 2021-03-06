﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        public static IEnumerable<T> Yield<T>(this T obj)
        {
            yield return obj;
        }

        public static IEnumerable<T> Infinite<T>(this T obj)
        {
            while(true)
            {
                yield return obj;
            }
        }

        public static IEnumerable<int> InfiniteRange(int start)
        {
            int i = start;
            while(true)
            {
                yield return i++;
            }
        }


        public static IEnumerable<T> StartWith<T>(this IEnumerable<T> source, T item)
        {
            return item.Yield().Concat(source);
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
            return source;
        }

        public static IEnumerable<(int index, T item)> Enumerated<T>(this IEnumerable<T> source)
        {
            return source.Select((t, i) => (i, t)).ToArray();
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params T[] items)
        {
            return source.Concat(items.AsEnumerable());
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }

        public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
        {
            return new Queue<T>(source);
        }

        public static Stack<T> ToStack<T>(this IEnumerable<T> source)
        {
            return new Stack<T>(source);
        }

        public static ConcurrentDictionary<K, T> ToConcurrentDictionary<K, T>(this IEnumerable<T> source, Func<T, K> keySelector)
        {
            return new ConcurrentDictionary<K, T>(source.ToDictionary(keySelector));
        }

        public static ConcurrentDictionary<K, E> ToConcurrentDictionary<K, T, E>(this IEnumerable<T> source, Func<T, K> keySelector, Func<T, E> elementSelector)
        {
            return new ConcurrentDictionary<K, E>(source.ToDictionary(keySelector, elementSelector));
        }


        public static ReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> source)
        {
            var builder = new ReadOnlyCollectionBuilder<T>(source);
            return builder.ToReadOnlyCollection();
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

        public static IEnumerable<T> Distinct<T, K>(this IEnumerable<T> source, Func<T, K> keySelector)
            where K: IEquatable<K>
        {
            return source.Distinct((item1, item2) => keySelector(item1).Equals(keySelector(item2)));
        }

        public static bool IsDistinct<T>(this IEnumerable<T> source, Func<T, T, bool> comparer)
        {
            return source.Distinct().HasSameElementsAs(source);
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

        public static IEnumerable<T> PadRight<T>(this IEnumerable<T> source, int length, Func<int, T> padding)
        {
            var i = 0;
            foreach (var item in source)
            {
                yield return item;
                i++;
            }

            for (; i< length; i++)
            {
                yield return padding(i);
            }
        }

        public static IEnumerable<T> PadLeft<T>(this IEnumerable<T> source, int length, Func<int, T> padding)
        {
            var items = source.ToList();

            for (int i = 0; i < length - items.Count; i++)
            {
                yield return padding(i);
            }

            foreach (var item in items)
            {
                yield return item;
            }
        }

        public static IEnumerable<T> ZipLongset<T1, T2, T>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, T> projection)
        {
            using (var iter1 = first.GetEnumerator())
            using (var iter2 = second.GetEnumerator())
            {
                while (iter1.MoveNext())
                {
                    if (iter2.MoveNext())
                    {
                        yield return projection(iter1.Current, iter2.Current);
                    }
                    else
                    {
                        yield return projection(iter1.Current, default(T2));
                    }
                }
                while (iter2.MoveNext())
                {
                    yield return projection(default(T1), iter2.Current);
                }
            }
        }

        public static IEnumerable<T> ZipThen<T1, T2, T>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, T> projection, Func<T1, T> then)
        {
            using (var iter1 = first.GetEnumerator())
            using (var iter2 = second.GetEnumerator())
            {
                while (iter1.MoveNext())
                {
                    if (iter2.MoveNext())
                    {
                        yield return projection(iter1.Current, iter2.Current);
                    }
                    else
                    {
                        yield return then(iter1.Current);
                    }
                }
            }
        }

        public static IEnumerable<T> ZipThen<T1, T2, T>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, T> projection, Func<T1, int, T> then)
        {
            int i = 0;
            using (var iter1 = first.GetEnumerator())
            using (var iter2 = second.GetEnumerator())
            {
                while (iter1.MoveNext())
                {
                    if (iter2.MoveNext())
                    {
                        yield return projection(iter1.Current, iter2.Current);
                    }
                    else
                    {
                        yield return then(iter1.Current, i++);
                    }
                }
            }
        }


        public static bool HasSameElementsAs<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            var sourceMap = source
                        .GroupBy(x => x)
                        .ToDictionary(x => x.Key, x => x.Count());

            var targetMap = target
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());

            var sourceIncludesTarget =
                sourceMap.Keys.All(x =>
                    targetMap.Keys.Contains(x) && sourceMap[x] == targetMap[x]);

            var targetIncludesSource =
                targetMap.Keys.All(x =>
                    sourceMap.Keys.Contains(x) && sourceMap[x] == targetMap[x]);

            return sourceIncludesTarget && targetIncludesSource;
        }

        public static IEnumerable<T> VerifyCount<T>(this IEnumerable<T> source, int count, Func<T> factory = null)
        {
            var counter = 0;
            foreach (var item in source)
            {
                if (counter >= count) yield break;
                yield return item;
                counter++;
            }

            if (factory == null) factory = () => default;

            while (counter < count)
            {
                yield return factory();
                counter++;
            }
        }

        public static bool SequenceEqual<T>(this IEnumerable<T> source, IEnumerable<T> target, Func<T, T, bool> isEqual)
        {
            var comparer = isEqual.ToEqualityComparer();
            return source.SequenceEqual(target, comparer);
        }

    }
}
