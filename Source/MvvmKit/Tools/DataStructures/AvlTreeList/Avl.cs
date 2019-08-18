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


        public static AvlTreeTarget<T> Target<T>(this AvlTreeNode<T> source, Func<AvlTreeNode<T>, AvlTreeNodeDirection> selector)
        {
            if (source == null) return new AvlTreeTarget<T> { Parent = null, ChildDirection = AvlTreeNodeDirection.Root };

            var current = source;

            AvlTreeTarget<T> res = null;

            while (res == null)
            {
                var nextDir = selector(current);
                var next = nextDir == AvlTreeNodeDirection.Left ? current.Left : current.Right;

                if (next == null)
                    res = new AvlTreeTarget<T> { Parent = current, ChildDirection = nextDir };

                current = next;
            }

            return res;
        }

        public static AvlTreeTarget<T> Target<T, K>(this AvlTreeNode<T> source, K initialPayload, 
            Func<AvlTreeNode<T>, K, (AvlTreeNodeDirection, K)> selector)
        {
            if (source == null) return new AvlTreeTarget<T> { Parent = null, ChildDirection = AvlTreeNodeDirection.Root };

            var current = source;
            var payload = initialPayload;

            AvlTreeTarget<T> res = null;

            while (res == null)
            {
                AvlTreeNodeDirection nextDir = AvlTreeNodeDirection.Right;
                (nextDir, payload) = selector(current, payload);

                var next = nextDir == AvlTreeNodeDirection.Left ? current.Left : current.Right;

                if (next == null)
                    res = new AvlTreeTarget<T> { Parent = current, ChildDirection = nextDir };

                current = next;
            }

            return res;
        }

        public static AvlTreeTarget<T> Target<T>(this AvlTree<T> source, Func<AvlTreeNode<T>, AvlTreeNodeDirection> selector)
        {
            return source.Root.Target(selector);
        }

        public static AvlTreeTarget<T> Target<T, K>(this AvlTree<T> source, K initialPayload,
            Func<AvlTreeNode<T>, K, (AvlTreeNodeDirection, K)> selector)
        {
            return source.Root.Target(initialPayload, selector);
        }

        public static AvlTreeNode<T> Find<T>(this AvlTreeNode<T> source, Func<AvlTreeNode<T>, AvlTreeNodeDirection> selector)
        {
            if (source == null) return null;

            var current = source;

            while (current != null)
            {
                var nextDir = selector(current);
                if (nextDir == AvlTreeNodeDirection.Root) return current;

                current = nextDir == AvlTreeNodeDirection.Left ? current.Left : current.Right;
            }

            return null;           
        }

        public static AvlTreeNode<T> Find<T, K>(this AvlTreeNode<T> source, K initialPayload, Func<AvlTreeNode<T>, K, (AvlTreeNodeDirection, K)> selector)
        {
            if (source == null) return null;

            var current = source;
            var payload = initialPayload;

            while (current != null)
            {
                var nextDir = AvlTreeNodeDirection.Root;

                (nextDir, payload) = selector(current, payload);
                if (nextDir == AvlTreeNodeDirection.Root) return current;

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
    }
}
