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

        public AvlTreeNode<T> RemoveAt(int index)
        {
            var node = this[index];
            Remove(node);
            return node;
        }

        public AvlTreeNode<T> Remove(AvlTreeNode<T> node)
        {
            InternalRemoveNode(node);
            return node;
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
            // newnode should be dettached from the tree
            Debug.Assert(newNode != null);
            Debug.Assert(newNode.Parent == null);
            Debug.Assert(newNode.Left == null);
            Debug.Assert(newNode.Right == null);
            Debug.Assert(newNode._tree == null);

            var parent = destination.Parent;
            _attachToParent(newNode, parent, destination.ChildDirection);

            _rebalanceAfterChange(parent);
            return newNode;
        }

        internal void InternalRemoveNode(AvlTreeNode<T> node)
        {
            var parent = node.Parent;
            var dleft = node.Left;
            var dright = node.Right;

            var direction = _directionOf(node);

            if ((dleft == null) && (dright == null))
            {
                /*
                 *      P           P  <- Balance from here
                 *      |-    =>    
                 *     node
                 */

                _dettachFromParent(node);
                _rebalanceAfterChange(parent);
            }
            else if ((dleft != null) && (dright == null))
            {
                /*
                 *      P           P  <- Balance from here
                 *      |-    =>    |+
                 *     node        dleft
                 *     /-
                 *    dleft
                 */
                _dettachFromParent(dleft);
                _dettachFromParent(node);
                _attachToParent(dleft, parent, direction);

                _rebalanceAfterChange(parent);
            }
            else if ((dleft == null) && (dright != null))
            {
                /*
                 *      P           P  <- Balance from here
                 *      |-    =>    |+
                 *     node        dright
                 *       \-
                 *       dright
                 */
                _dettachFromParent(dright);
                _dettachFromParent(node);
                _attachToParent(dright, parent, direction);

                _rebalanceAfterChange(parent);
            }
            else
            {
                // both node.left and node.right are full
                // find successor descendent - One step to the right, and as many steps to the left as possible
                var successor = dright;
                while (successor.Left != null) successor = successor.Left;

                var sparent = successor.Parent;
                var sright = successor.Right; // may be null

                // two cases. One of them is the the successor is the direct right child of the deleted node
                // the second is that there is a deeper successor
                if (successor == dright)
                {
                    //           parent                       parent
                    //              |-                           |+
                    //            node            ==>        dright (Successor) <-Rebalance from here
                    //            /- \-                         /+
                    //       dleft  dright (Successor)     dleft

                    _dettachFromParent(dright);
                    _dettachFromParent(node);
                    _dettachFromParent(dleft);
                    _attachToParent(dright, parent, direction);
                    _attachToParent(dleft, dright, AvlTreeNodeDirection.Left);
                    _rebalanceAfterChange(dright);

                } else
                {
                    //        parent                          parent
                    //          |-                               |+
                    //         node                          successor
                    //        /-   \-                          /+   \+
                    //     dleft  dright                    dleft  dright
                    //             /                                 /
                    //            ...               ==>             ...
                    //            /                                 /
                    //          sparent                         sparent <-Rebalance from here
                    //            /-                               /+
                    //          successor                       sright
                    //            \-
                    //           sright

                    _dettachFromParent(sright);
                    _dettachFromParent(successor);
                    _dettachFromParent(dright);
                    _dettachFromParent(dleft);
                    _dettachFromParent(node);

                    _attachToParent(successor, parent, direction);
                    _attachToParent(dleft, successor, AvlTreeNodeDirection.Left);
                    _attachToParent(dright, successor, AvlTreeNodeDirection.Right);
                    _attachToParent(sright, sparent, AvlTreeNodeDirection.Left);

                    _rebalanceAfterChange(sparent);
                }
            }

            _recalc(node); // just to make sure height and size are correct
            Debug.Assert(node.Left == null);
            Debug.Assert(node.Right == null);
            Debug.Assert(node.Parent == null);
            Debug.Assert(node._tree == null);
            Debug.Assert(node.Height == 1);
            Debug.Assert(node.Size == 1);
            Debug.Assert(node.Balance == 0);
        }

        private void _attachToParent(AvlTreeNode<T> node, AvlTreeNode<T> parent, AvlTreeNodeDirection direction)
        {
            // if node != null, node.parent should be free
            Debug.Assert((node == null) || (node.Parent == null));  

            // if direction is not root, parent should contain something
            Debug.Assert((direction == AvlTreeNodeDirection.Root) || (parent != null));

            // if direction is root, parent should be null
            Debug.Assert((direction != AvlTreeNodeDirection.Root) || (parent == null));

            // if direction is left, parent.left should be free
            Debug.Assert((direction != AvlTreeNodeDirection.Left) || (parent.Left == null));

            // if direction is right, parent.right should be free
            Debug.Assert((direction != AvlTreeNodeDirection.Right) || (parent.Right == null));

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
            if (node == null) return;

            if (node.Parent == null)
            {
                _unsetRoot(node);
            } else if (node.Parent.Left == node)
            {
                node.Parent.Left = null;
                node.Parent = null;
            } else
            {
                node.Parent.Right = null;
                node.Parent = null;
            }
        }

        private void _setAsRoot(AvlTreeNode<T> node)
        {
            Debug.Assert(Root == null);
            Debug.Assert(node.Parent == null);
            Debug.Assert(node._tree == null);

            Root = node;
            if (node != null)
            {
                node._tree = this;
            }
        }

        private void _unsetRoot(AvlTreeNode<T> node)
        {
            Debug.Assert(Root == node);
            Debug.Assert((node == null) || (node.Parent == null));
            Debug.Assert((node == null) || (node._tree == this));

            Root = null;
            if (node != null) node._tree = null;            
        }


        private void _rebalanceAfterChange(AvlTreeNode<T> start)
        {
            while (start != null)
            {
                var direction = _directionOf(start);

                _recalc(start);
                var parent = start.Parent;
                var balanced = _doBalance(start);

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
                    _rotateLeft(node.Left);
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
         *   |-           |+
		 *   A            B
		 *  / \-        +/ \
		 * AL  B   ->   A   BR
		 *   -/ \      / \+
		 *   C   BR    0   C
         *   
         *   - marks connections that are dettached
         *   + marks connections that are created
		 */
        private AvlTreeNode<T> _rotateLeft(AvlTreeNode<T> a)
        {
            AvlTreeNode<T> b = a.Right;
            AvlTreeNode<T> parent = a.Parent; // may be null if A was root
            AvlTreeNode<T> c = b.Left; // may be null

            var direction = _directionOf(a);

            Debug.Assert(b != null);

            // a.Left remains so no need to change this
            // b.Right remains so no need to change this

            _dettachFromParent(a); // a from parent
            _dettachFromParent(b); // b from a
            _dettachFromParent(c); // c from b

            _attachToParent(c, a, AvlTreeNodeDirection.Right);
            _attachToParent(a, b, AvlTreeNodeDirection.Left);
            _attachToParent(b, parent, direction);

            _recalc(a);
            _recalc(b);
            return b;
        }

        /*     Parent     Parent
         *     |-         |+
		 *     B          A
		 *   -/ \        / \+
		 *   A   BL  -> AL   B
		 *  / \-          +/ \
		 * AL   C          C   BR
         * 
         *   - marks connections that are dettached
         *   + marks connections that are created
		 */

        private AvlTreeNode<T> _rotateRight(AvlTreeNode<T> b)
        {
            AvlTreeNode<T> a = b.Left;
            AvlTreeNode<T> parent = b.Parent; // may be null if B was root
            AvlTreeNode<T> c = a.Right; // may be null

            var direction = _directionOf(b);

            Debug.Assert(a != null);

            // a.Left remains so no need to change this
            // b.Right remains so no need to change this

            _dettachFromParent(b); // b from parent
            _dettachFromParent(a); // a from b
            _dettachFromParent(c); // c from a

            _attachToParent(c, b, AvlTreeNodeDirection.Left);
            _attachToParent(b, a, AvlTreeNodeDirection.Right);
            _attachToParent(a, parent, direction);

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
