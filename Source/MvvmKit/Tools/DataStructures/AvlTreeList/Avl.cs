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

        internal static AvlTreeTarget<T> Target<T>(this AvlTree<T> source, Func<AvlTreeNode<T>, AvlTreeNodeDirection> selector)
        {
            if (source.Root == null) return new AvlTreeTarget<T> { Parent = null, ChildDirection = AvlTreeNodeDirection.Root };

            var current = source.Root;

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

        internal static AvlTreeTarget<T> Target<T, K>(this AvlTree<T> source, K initialPayload, 
            Func<AvlTreeNode<T>, K, (AvlTreeNodeDirection, K)> selector)
        {
            if (source.Root == null) return new AvlTreeTarget<T> { Parent = null, ChildDirection = AvlTreeNodeDirection.Root };

            var current = source.Root;
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

    }
}
