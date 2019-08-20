using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    /// <summary>
    /// AvlTree is a balanced tree data structure. It stores and organizes AvlTreeNode instances according to index. If you are looking for a data structure that sorts 
    /// items by their value, refer to the SortedAvlTree data structure. 
    /// Some key assumptions: 
    /// - You can create tree nodes and insert them into the tree, or you can let the tree create them for you.
    /// - You can remove nodes from the tree and insert them into other trees.
    /// - The nodes are maintained in an order that the user manually controls. When the user adds a node to the tree, he must specify where to place it
    /// - Insert, Remove, Access by index, IndexOf, are O(log(N)) operations
    /// - Clear, Reset (collection), Enumerate are O(N) operations
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class OrderedAvlTree<T> : AvlTree<T>
    {
        public OrderedAvlTree()
        {
        }

        public OrderedAvlTree(IEnumerable<T> collection)
        {
            Reset(collection);
        }

        public AvlTreeNode<T> AddFirst(T item)
        {
            return InsertAt(0, item);
        }

        public AvlTreeNode<T> AddLast(T item)
        {
            return InsertAt(Count, item);
        }

        public AvlTreeNode<T> Add(T item)
        {
            return AddLast(item);
        }

        public AvlTreeNode<T> InsertAt(int index, T item)
        {
            if ((index < 0) || (index > Count))
                throw new IndexOutOfRangeException();

            return _insertInRelativeIndex(Root, index, item);
        }

        public AvlTreeNode<T> AddAfter(AvlTreeNode<T> anchor, T item)
        {
            if (anchor == null)
                throw new ArgumentNullException(nameof(anchor));

            var indexOfAnchor = anchor.LeftSize; // index of anchor relative to it's own subtree == the number of items at its left

            return _insertInRelativeIndex(anchor, indexOfAnchor + 1, item);
        }

        public AvlTreeNode<T> AddBefore(AvlTreeNode<T> anchor, T item)
        {
            if (anchor == null)
                throw new ArgumentNullException(nameof(anchor));
            var indexOfAnchor = anchor.LeftSize; // index of anchor relative to it's own subtree == the number of items at its left

            return _insertInRelativeIndex(anchor, indexOfAnchor, item);
        }

        public AvlTreeNode<T> Find(T item)
        {
            return this.FirstOrDefault(node => Equals(node.Item, item));
        }

        private AvlTreeNode<T> _insertInRelativeIndex(AvlTreeNode<T> anchor, int index, T item)
        {
            Debug.Assert(index >= 0);
            Debug.Assert((anchor != null) || (index == 0)); // if anchor is null, index must be 0
            Debug.Assert((anchor == null) || (index <= anchor.Size)); // if anchor is not null, index must be less than or queal to 
                                                                      // the size of the subtree

            var newNode = new AvlTreeNode<T>(item);
            if (anchor == null)
                return InternalInsertNode(AvlTreeEdge<T>.Root, newNode);

            var edge = anchor.FindFree(index, (node, idx) =>
            {
                int leftSize = node.LeftSize;
                if (idx <= leftSize) return (node.EdgeLeft(), idx);
                return (node.EdgeRight(), idx - leftSize - 1);
            });

            return InternalInsertNode(edge, newNode);
        }

        public override void Reset(IEnumerable<T> collection)
        {
            InternalReset(collection.ToArray());
        }

        internal override void OnItemChanged(AvlTreeNode<T> node)
        {
            // nothing to do
        }

        protected override void CheckStructureOfNode(AvlTreeNode<T> node)
        {
            // nothing to check
        }
    }
}
