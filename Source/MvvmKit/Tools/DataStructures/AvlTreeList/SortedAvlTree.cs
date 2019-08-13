using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    /// <summary>
    /// SortedAvlTree is a balanced tree data structure. It stores and organizes AvlTreeNode instances according to their values. If you are looking for a data structure that stores 
    /// items by a manually set order, refer to the AvlTree data structure.
    /// Some key assumptions: 
    /// - You can create tree nodes and insert them into the tree, or you can let the tree create them for you.
    /// - You can remove nodes from the tree and insert them into other trees.
    /// - The nodes are maintained in an order that is implied by the item values. When the user adds a node to the tree, The value of the item of the node determines its position
    /// - Insert, Remove, Access by index, IndexOf, are O(log(N)) operations
    /// - Clear, Reset (collection), Enumerate are O(N) operations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortedAvlTree<T> : AvlTree<T>
    {
        private IComparer<T> _comparer;

        public SortedAvlTree(IComparer<T> comparer)
        {
            _comparer = comparer;
        }

        public SortedAvlTree(IEnumerable<T> collection, IComparer<T> comparer)
            : this(comparer)
        {
            Reset(collection);
        }

        public SortedAvlTree()
            :this(Comparer<T>.Default)
        {
        }

        public SortedAvlTree(IEnumerable<T> collection)
            :this(collection, Comparer<T>.Default)
        {
        }



        public AvlTreeNode<T> Add(T item)
        {
            return _add(new AvlTreeNode<T>(item));
        }

        private AvlTreeNode<T> _add(AvlTreeNode<T> node)
        {
            var item = node.Item;
            var target = this.Target(n => _comparer.Compare(item, n.Item) > 0
                ? AvlTreeNodeDirection.Right
                : AvlTreeNodeDirection.Left);

            return InternalInsertNode(target, node);
        }


        public override void Reset(IEnumerable<T> collection)
        {
            throw new NotImplementedException();
        }

        internal override void OnItemChanged(AvlTreeNode<T> node)
        {
            // remove and reinsert the item so it ends up in the right place
            InternalRemoveNode(node);
            _add(node);
        }

        internal override void CheckStructureOfNode(AvlTreeNode<T> node)
        {
            if (node.Left != null)
            {
                var comp = _comparer.Compare(node.Item, node.Left.Item);
                if (comp < 0) throw new SystemException("Sorted tree, node item smaller than its left child item");
            }

            if (node.Right != null)
            {
                var comp = _comparer.Compare(node.Item, node.Right.Item);
                if (comp > 0) throw new SystemException("Sorted tree, node item larger than its right child item");
            }

        }
    }
}
