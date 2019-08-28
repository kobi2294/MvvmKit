using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class Avl
    {
        public static OrderedAvlTree<T> ToAvlTree<T>(this IEnumerable<T> values)
        {
            return new OrderedAvlTree<T>(values);
        }

        public static SortedAvlTree<T> ToSortedAvlTree<T>(this IEnumerable<T> values)
        {
            return new SortedAvlTree<T>(values);
        }

        public static SortedAvlTree<T> ToSortedAvlTree<T>(this IEnumerable<T> values, IComparer<T> comparer)
        {
            return new SortedAvlTree<T>(values, comparer);
        }

        public static SortedAvlTree<T> ToSortedAvlTree<T>(this IEnumerable<T> values, Func<T, T, int> comparer)
        {
            return new SortedAvlTree<T>(values, comparer.ToComparer());
        }

        public static SortedAvlTree<T> ToSortedAvlTree<T, K>(this IEnumerable<T> values, Func<T, K> orderBy)
        {
            Func<T, T, int> comparer = (t1, t2) => Comparer<K>.Default.Compare(orderBy(t1), orderBy(t2));

            return new SortedAvlTree<T>(values, comparer.ToComparer());
        }



        public static string ToVisualString<T>(this AvlTreeNode<T> source, string textFormat = "{0}", int spacing = 2, int topMargin = 1, int leftMargin = 1)
        {
            var console = new StringConsole();
            source.PrintToConsole(console, textFormat, spacing, topMargin, leftMargin);
            return console.ToString();
        }

        public static string ToVisualString<T>(this AvlTree<T> source, string textFormat = "{0}", int spacing = 2, int topMargin = 1, int leftMargin = 1)
        {
            return source.Root.ToVisualString(textFormat, spacing, topMargin, leftMargin);
        }



        public static AvlTreeEdge<T> FindFree<T>(this AvlTreeNode<T> source, Func<AvlTreeNode<T>, AvlTreeEdge<T>> selector)
        {
            if (source == null) return AvlTreeEdge<T>.Root;

            var current = source;

            AvlTreeEdge<T> res = null;

            while (res == null)
            {
                var edge = selector(current);
                var next = edge.Target();

                if (next == null)
                    res = edge;

                current = next;
            }

            return res;
        }

        public static AvlTreeEdge<T> FindFree<T, K>(this AvlTreeNode<T> source, K initialPayload, 
            Func<AvlTreeNode<T>, K, (AvlTreeEdge<T>, K)> selector)
        {
            if (source == null) return AvlTreeEdge<T>.Root;

            var current = source;
            var payload = initialPayload;

            AvlTreeEdge<T> res = null;

            while (res == null)
            {
                AvlTreeEdge<T> edge = null;
                (edge, payload) = selector(current, payload);

                var next  = edge.Target();

                if (next == null)
                    res = edge;

                current = next;
            }

            return res;
        }

        public static AvlTreeEdge<T> FindFree<T>(this AvlTree<T> source, Func<AvlTreeNode<T>, AvlTreeEdge<T>> selector)
        {
            return source.Root.FindFree(selector);
        }

        public static AvlTreeEdge<T> FindFree<T, K>(this AvlTree<T> source, K initialPayload,
            Func<AvlTreeNode<T>, K, (AvlTreeEdge<T>, K)> selector)
        {
            return source.Root.FindFree(initialPayload, selector);
        }


        public static AvlTreeNode<T> Find<T, K>(this AvlTreeNode<T> source, K initialPayload, Func<AvlTreeNode<T>, K, (AvlTreeNodeDirection, K)> selector)
        {
            if (source == null) return null;

            var current = source;
            var payload = initialPayload;

            while (current != null)
            {
                var nextDir = AvlTreeNodeDirection.None;

                (nextDir, payload) = selector(current, payload);
                if (nextDir == AvlTreeNodeDirection.None) return current;

                current = nextDir == AvlTreeNodeDirection.Left ? current.Left : current.Right;
            }

            return null;
        }

        public static AvlTreeNode<T> Find<T>(this AvlTreeNode<T> source, Func<AvlTreeNode<T>, AvlTreeNodeDirection> selector)
        {
            if (source == null) return null;

            var current = source;

            while (current != null)
            {
                var nextDir = selector(current);
                if (nextDir == AvlTreeNodeDirection.None) return current;

                current = nextDir == AvlTreeNodeDirection.Left ? current.Left : current.Right;
            }

            return null;           
        }

        public static AvlTreeNode<T> Find<T>(this AvlTree<T> source, Func<AvlTreeNode<T>, AvlTreeNodeDirection> selector)
        {
            return source.Root.Find(selector);
        }

        public static AvlTreeNode<T> Find<T, K>(this AvlTree<T> source, K initialPayload, Func<AvlTreeNode<T>, K, (AvlTreeNodeDirection, K)> selector)
        {
            return source.Root.Find(initialPayload, selector);
        }


        /// <summary>
        /// Returns a path of tree edges, starting from the source node, while traversing the nodes according to a selector
        /// function that chooses the next edge to walk on. The path ends when the selector returns null. You may also use 
        /// a payload that is carried from step to step, while allowing the selector to set a new payload for the next step.
        /// </summary>
        /// <typeparam name="T">The type of tree nodes</typeparam>
        /// <typeparam name="K">The type of payload</typeparam>
        /// <param name="source">The starting point of the path</param>
        /// <param name="initialPayload">The starting value of the payload</param>
        /// <param name="selector">A function that selects the next edge and the next payload to be carried onto the next step</param>
        /// <returns></returns>
        public static IEnumerable<AvlTreeEdge<T>> Path<T, K>(
            this AvlTreeNode<T> source, K initialPayload, 
            Func<AvlTreeNode<T>, K, (AvlTreeEdge<T>, K)> selector)
        {
            if (source == null)
            {
                yield return AvlTreeEdge<T>.Root;
                yield break;
            }

            var current = source;
            var payload = initialPayload;

            while (current != null)
            {
                AvlTreeEdge<T> edge = null;
                (edge, payload) = selector(current, payload);

                if (edge == null)
                {
                    yield break;
                }
                current = edge.Target();
            }

            // if we got here, the last edge is pointing towards null, so we can not go any further
        }

        public static IEnumerable<AvlTreeEdge<T>> Path<T>(
            this AvlTreeNode<T> source, Func<AvlTreeNode<T>, AvlTreeEdge<T>> selector)
        {
            // use a dummy payload (0)
            return source.Path(0, (node, p) => (selector(node), p));
        }

        public static IEnumerable<AvlTreeEdge<T>> Path<T, K>(
            this AvlTree<T> source, K initialPayload,
            Func<AvlTreeNode<T>, K, (AvlTreeEdge<T>, K)> selector)
        {
            return source.Root.Path(initialPayload, selector);
        }
        public static IEnumerable<AvlTreeEdge<T>> Path<T>(
            this AvlTree<T> source, Func<AvlTreeNode<T>, AvlTreeEdge<T>> selector)
        {
            return source.Root.Path(selector);
        }


        public static AvlTreeNode<T> Target<T>(this IEnumerable<AvlTreeEdge<T>> source)
        {
            var last = source.LastOrDefault();

            return last?.Target();
        }

        public static IEnumerable<AvlTreeNode<T>> Nodes<T>(this IEnumerable<AvlTreeEdge<T>> source)
        {
            var first = source.FirstOrDefault()?.Source;

            return first.Yield().Concat(
                source.Select(edge => edge.Target()).Where(n => n != null)
                );
        }

    }
}
