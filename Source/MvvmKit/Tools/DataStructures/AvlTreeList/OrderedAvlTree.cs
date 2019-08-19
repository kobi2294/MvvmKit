using System;
using System.Collections.Generic;
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

        public AvlTreeNode<T> AddFirst(T item) => throw new NotImplementedException();

        public AvlTreeNode<T> AddLast(T item) => throw new NotImplementedException();

        public AvlTreeNode<T> InsertAt(int index, T item) => throw new NotImplementedException();

        public AvlTreeNode<T> AddAfter(AvlTreeNode<T> anchor, T item) => throw new NotImplementedException();

        public AvlTreeNode<T> AddBefore(AvlTreeNode<T> anchor, T item) => throw new NotImplementedException();

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
