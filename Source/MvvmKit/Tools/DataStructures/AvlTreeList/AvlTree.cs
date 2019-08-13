using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class AvlTree<T>
    {
        public AvlTree()
        {}

        public int Count => Root != null ? Root.Size : 0;

        public int Height => Root != null ? Root.Height : 0;

        public AvlTreeNode<T> Root { get; internal set; }

        public AvlTreeNode<T> this[int index] => rec_getNodeAt(Root, index);

        public int IndexOf(AvlTreeNode<T> node) => _indexOf(node);

        public T RemoveAt(int index)
        {
            var node = this[index];
            var value = Remove(node);
            return value;
        }

        public T Remove(AvlTreeNode<T> node)
        {
            var value = node.Item;
            InternalRemoveNode(node);
            return value;
        }

        public AvlTreeNode<T> Clear()
        {
            var root = Root;
            _dettachFromParent(Root);
            return root;
        }

        public abstract void Reset(IEnumerable<T> collection);

        public void CheckStructure()
        {
            _checkStructure();
        }


        internal abstract void OnItemChanged(AvlTreeNode<T> node);

        internal abstract void CheckStructureOfNode(AvlTreeNode<T> node);

        private int _leftSize(AvlTreeNode<T> node) => node.Left == null ? 0 : node.Left.Size;

        private int _rightSize(AvlTreeNode<T> node) => node.Right == null ? 0 : node.Right.Size;

        private int _size(AvlTreeNode<T> node) => node == null ? 0 : node.Size;

        private AvlTreeNode<T> rec_getNodeAt(AvlTreeNode<T> node, int index)
        {
            var nodeSize = _size(node);
            if ((index < 0) || (index >= nodeSize)) throw new ArgumentOutOfRangeException(nameof(index));

            int leftSize = _leftSize(node);
            if (index < leftSize) return rec_getNodeAt(node.Left, index);
            if (index > leftSize) return rec_getNodeAt(node.Right, index - leftSize - 1);

            return node;
        }

        private int _indexOf(AvlTreeNode<T> node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            // the index of a node is actually that number of nodes that are "before" it. That includes 
            // left children, and left siblings, including ancestors that the node is "right child" of.


            var count = _leftSize(node);
            var cur = node;
            while (cur.Parent != null)
            {
                if (cur.Parent.Right == cur) // if cur is the right child of the parent
                {
                    count = count + _leftSize(cur.Parent) + 1; // count all the left descendants of the parent, and the parent itself
                }
                cur = cur.Parent;
            }

            return count;
        }

        private AvlTreeNodeDirection _directionOf(AvlTreeNode<T> node)
        {
            if (node.Parent == null) return AvlTreeNodeDirection.Root;
            if (node.Parent.Left == node) return AvlTreeNodeDirection.Left;
            if (node.Parent.Right == node) return AvlTreeNodeDirection.Right;

            throw new InvalidOperationException("Node has no relation to its parent");
        }

        internal AvlTreeNode<T> InternalInsertNode(AvlTreeTarget<T> destination, AvlTreeNode<T> newNode)
        {
            Debug.Assert(newNode.Parent == null);
            Debug.Assert(newNode.Left == null);
            Debug.Assert(newNode.Right == null);
            Debug.Assert(newNode._tree == null);

            var parent = destination.Parent;
            newNode.Parent = parent;

            if (parent == null)
            {
                Debug.Assert(Root == null);
                _setAsRoot(newNode);
            } else if (destination.ChildDirection == AvlTreeNodeDirection.Left)
            {
                Debug.Assert(parent.Left == null);
                parent.Left = newNode;
            }
            else
            {
                Debug.Assert(parent.Right == null);
                parent.Right = newNode;
            }

            _rebalanceAfterChange(parent);
            return newNode;
        }

        internal void InternalRemoveNode(AvlTreeNode<T> node)
        {
            var parent = node.Parent;
            var direction = _directionOf(node);

            if ((node.Left == null) && (node.Right == null))
            {
                _dettachFromParent(node);
                _rebalanceAfterChange(parent);
                node.Parent = null;
            }
            else if ((node.Left != null) && (node.Right == null))
            {
                _attachToParent(node.Left, parent, direction);
                node.Left = null;
                node.Parent = null;
                _rebalanceAfterChange(parent);

            }
            else if ((node.Left == null) && (node.Right != null))
            {
                _attachToParent(node.Right, parent, direction);
                node.Right = null;
                node.Parent = null;
                _rebalanceAfterChange(parent);
            }
            else
            {
                // both node.left and node.right are full
                // find successor descendent
                var temp = node.Right;
                while (temp.Left != null) temp = temp.Left;

                // temp is now the successor node

                // two cases. One of them is the the successor is the direct right child of the deleted node
                // the second is that there is a deeper successor
                if (temp == node.Right)
                {
                    //          D-Parent                    D-Parent
                    //              |                           |
                    //           Deleted            ==>     D-Right
                    //            /  \                          /
                    //       D-Left  D-Right                D-Left
                    var dleft = node.Left;
                    var dright = node.Right;

                    node.Left = null;
                    node.Right = null;
                    node.Parent = null;

                    _attachToParent(dright, parent, direction);
                    _attachToParent(dleft, dright, AvlTreeNodeDirection.Left);
                    _rebalanceAfterChange(dright);

                } else
                {
                    //      D-Parent                         D-Parent
                    //          |                                |
                    //       Deleted                          Succesor
                    //       /     \                            /   \
                    //   D-Left D-Right                     D-Left  D-Right
                    //             /                                     /
                    //            ...               ==>                ...
                    //            /                                     /
                    //          Suc-Parent                          S-Parent
                    //            /                                     /
                    //          Succesor                            S-Right
                    //            \
                    //           Suc-Right

                    var dleft = node.Left;
                    var dright = node.Right;

                    var s = temp;
                    var sparent = s.Parent;
                    var sright = s.Right; // may be null

                    node.Parent = null;
                    node.Left = null;
                    node.Right = null;

                    _attachToParent(s, parent, direction);
                    _attachToParent(dleft, s, AvlTreeNodeDirection.Left);
                    _attachToParent(dright, s, AvlTreeNodeDirection.Right);

                    _attachToParent(sright, sparent, AvlTreeNodeDirection.Left);
                    _rebalanceAfterChange(sparent);
                }
            }

            node.Height = 1;
            node.Size = 1;
        }

        private void _attachToParent(AvlTreeNode<T> node, AvlTreeNode<T> parent, AvlTreeNodeDirection direction)
        {
            if (parent == null)
            {
                _setAsRoot(node);
            } else if (direction == AvlTreeNodeDirection.Left)
            {
                parent.Left = node;
                if (node != null) node.Parent = parent;
            } else
            {
                parent.Right = node;
                if (node != null) node.Parent = parent;
            }
        }

        private void _dettachFromParent(AvlTreeNode<T> node)
        {
            if (node.Parent == null)
            {
                _setAsRoot(null);
            } else if (node.Parent.Left == node)
            {
                node.Parent.Left = null;
            } else
            {
                node.Parent.Right = null;
            }
        }

        private void _setAsRoot(AvlTreeNode<T> node)
        {
            if (Root != null)
                Root._tree = null;

            Root = node;

            if (node != null)
            {
                node._tree = this;
                node.Parent = null;
            }
        }

        private void _rebalanceAfterChange(AvlTreeNode<T> start)
        {
            while (start != null)
            {
                var direction = _directionOf(start);

                _recalc(start);
                var parent = start.Parent;
                var balanced = _doBalance(start);

                _attachToParent(balanced, parent, direction);

                start = parent;
            }
        }

        private AvlTreeNode<T> _doBalance(AvlTreeNode<T> node)
        {
            int bal = node.Balance;

            Debug.Assert(Math.Abs(bal) <= 2);
            AvlTreeNode<T> res = node;

            if (bal == -2)
            {
                Debug.Assert(Math.Abs(node.Left.Balance) <= 1);
                if (node.Left.Balance == 1)
                    node.Left = _rotateLeft(node.Left);
                res = _rotateRight(node);
            }
            else if (bal == 2)
            {
                Debug.Assert(Math.Abs(node.Right.Balance) <= 1);
                if (node.Right.Balance == -1)
                    node.Right = _rotateRight(node.Right);
                res = _rotateLeft(node);
            }

            return res;
        }

        /*   Parent       Parent
         *   |            |
		 *   A            B
		 *  / \          / \
		 * 0   B   ->   A   2
		 *    / \      / \
		 *   1   2    0   1
         *   
		 */
        private AvlTreeNode<T> _rotateLeft(AvlTreeNode<T> a)
        {
            Debug.Assert(a.Right != null);
            AvlTreeNode<T> b = a.Right;
            AvlTreeNode<T> parent = a.Parent;

            // a.Left remains 0 so no need to change this
            // b.Right remains 2 so no need to change this
            // b.Left (1) becomes a.Right

            a.Right = b.Left; // 1
            b.Left = a;

            a.Parent = b;
            if (a.Right != null) a.Right.Parent = a;
            b.Parent = parent;

            _recalc(a);
            _recalc(b);
            return b;
        }

        /*     Parent     Parent
         *     |          |
		 *     B          A
		 *    / \        / \
		 *   A   2  ->  0   B
		 *  / \            / \
		 * 0   1          1   2
         * 
		 */

        private AvlTreeNode<T> _rotateRight(AvlTreeNode<T> b)
        {
            Debug.Assert(b.Left != null);
            AvlTreeNode<T> a = b.Left;
            AvlTreeNode<T> parent = b.Parent;

            // a.Left remains 0 so no need to change this
            // b.Right remains 2 so no need to change this
            // a.Right (1) becomes b.Left
            b.Left = a.Right; // 1 
            a.Right = b;

            b.Parent = a;
            if (b.Left != null) b.Left.Parent = b;
            a.Parent = parent;

            _recalc(b);
            _recalc(a);
            return a;
        }

        // Needs to be called every time the left or right subtree is changed.
        // Assumes the left and right subtrees have the correct values computed already.
        private void _recalc(AvlTreeNode<T> node)
        {
            Debug.Assert(node != null);

            node.Height = Math.Max(node.Left?.Height ?? 0, node.Right?.Height ?? 0) + 1;
            node.Size = (node.Left?.Size ?? 0) + (node.Right?.Size ?? 0) + 1;

            Debug.Assert((node.Height >= 1) && (node.Size >= 1));
        }

        private void rec_checkStructure(AvlTreeNode<T> node, ISet<AvlTreeNode<T>> visitedNodes)
        {
            if (node == null)
                return;

            if (!visitedNodes.Add(node))
                throw new SystemException("AVL tree structure violated: Not a tree");

            CheckStructureOfNode(node);

            rec_checkStructure(node.Left, visitedNodes);
            rec_checkStructure(node.Right, visitedNodes);

            if ((node.Left != null) && (node.Left.Parent != node))
                throw new SystemException("AVL tree structure violated: node.Left.Parent != node");

            if ((node.Right != null) && (node.Right.Parent != node))
                throw new SystemException("AVL tree structure violated: node.Right.Parent != node");

            if (node.Height != Math.Max((node.Left?.Height ?? 0), (node.Right?.Height ?? 0)) + 1)
                throw new SystemException("AVL tree structure violated: Incorrect cached height");

            if (node.Size != (node.Left?.Size ?? 0) + (node.Right?.Size ?? 0) + 1) 
                throw new SystemException("AVL tree structure violated: Incorrect cached size");

            if (Math.Abs(node.Balance) > 1)
                throw new SystemException("AVL tree structure violated: Height imbalance");

        }

        private void _checkStructure()
        {
            rec_checkStructure(Root, new HashSet<AvlTreeNode<T>>());
        }


    }
}
