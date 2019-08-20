using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class AvlTree<T>: IEnumerable<AvlTreeNode<T>>
    {
        private int _version = 0;

        public AvlTree()
        {}

        public int Count => Root != null ? Root.Size : 0;

        public int Height => Root != null ? Root.Height : 0;

        public AvlTreeNode<T> Root { get; internal set; }

        public AvlTreeNode<T> this[int index] => rec_getNodeAt(Root, index);

        public int IndexOf(AvlTreeNode<T> node) => _indexOf(node);

        public AvlTreeNode<T> SuccessorOf(AvlTreeNode<T> node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            return InternalSuccessor(node);
        }

        public AvlTreeNode<T> PredecessorOf(AvlTreeNode<T> node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            return InternalPredecessor(node);
        }

        public AvlTreeNode<T> FirstInSubtree(AvlTreeNode<T> node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            return InternalFirstInSubtree(node);
        }

        public AvlTreeNode<T> LastInSubtree(AvlTreeNode<T> node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            return InternalLastInSubtree(node);
        }

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
            return InternalClear();
        }

        public IEnumerable<T> Items => this.Select(node => node.Item);

        public IEnumerator<AvlTreeNode<T>> GetEnumerator()
        {
            return InternalEnumerate();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract void Reset(IEnumerable<T> collection);

        public void CheckStructure()
        {
            _checkStructure();
        }


        internal abstract void OnItemChanged(AvlTreeNode<T> node);

        protected abstract void CheckStructureOfNode(AvlTreeNode<T> node);

        internal AvlTreeNode<T> InternalSuccessor(AvlTreeNode<T> node)
        {
            Debug.Assert(node != null);

            // 3 cases:
            // case 1 - there is a right subtree, in that case, the successor is the first in the right sub tree.
            // case 2 - there is no right subtree, but there is a parent - the successor is the first parent "on the right" - so it's child is a left child
            // case 3 - no parent, or we keep clibing up parents where we are the "child on the right" - there is no successor

            // case 1
            if (node.Right != null) return InternalFirstInSubtree(node.Right);

            var cur = node;
            while (cur.Parent != null)
            {
                // case 2
                // if we are the left child, so the parent is on our right - this is the successor
                if (cur.Direction == AvlTreeNodeDirection.Left) return cur.Parent;

                // else - keep climbing up
                cur = cur.Parent;
            }

            // case 3 - no more parents - no successor
            return null;            
        }

        internal AvlTreeNode<T> InternalPredecessor(AvlTreeNode<T> node)
        {
            Debug.Assert(node != null);

            // 3 cases:
            // case 1 - there is a left subtree, in that case, the predecessor is the last in the left sub tree.
            // case 2 - there is no left subtree, but there is a parent - the predecessor is the first parent "on the left" - so it's child is a right child
            // case 3 - no parent, or we keep clibing up parents where we are the "child on the left" - there is no predecessor

            // case 1
            if (node.Left != null) return InternalLastInSubtree(node.Left);

            var cur = node;
            while (cur.Parent != null)
            {
                // case 2
                // if we are the right child, so the parent is on our left - this is the predecessor
                if (cur.Direction == AvlTreeNodeDirection.Right) return cur.Parent;

                // else - keep climbing up
                cur = cur.Parent;
            }

            // case 3 - no more parents - no predecessor
            return null;
        }

        internal AvlTreeNode<T> InternalFirstInSubtree(AvlTreeNode<T> node)
        {
            Debug.Assert(node != null);

            var cur = node;
            while (cur.Left != null) cur = cur.Left;
            return cur;            
        }

        internal AvlTreeNode<T> InternalLastInSubtree(AvlTreeNode<T> node)
        {
            Debug.Assert(node != null);

            var cur = node;
            while (cur.Right != null) cur = cur.Right;
            return cur;
        }

        internal IEnumerator<AvlTreeNode<T>> InternalEnumerate()
        {
            var version = _version;
            var all = _enumerate(Root);
            foreach (var item in all)
            {
                if (_version != version)
                    throw new InvalidOperationException("Tree was modified; enumeration operation may not execute.");
                yield return item;
            }
        }

        internal AvlTreeNode<T> InternalInsertNode(AvlTreeEdge<T> edge, AvlTreeNode<T> newNode)
        {
            Debug.Assert(edge.IsChildOrRoot);

            if (Count == int.MaxValue)
                throw new InvalidOperationException("Maximum size reached");

            _version++;
            // newnode should be dettached from the tree
            Debug.Assert(newNode != null);
            Debug.Assert(newNode.Parent == null);
            Debug.Assert(newNode.Left == null);
            Debug.Assert(newNode.Right == null);
            Debug.Assert(newNode._tree == null);

            _attachToParentEdge(newNode, edge);

            _rebalanceAfterChange(edge.Source);
            return newNode;
        }

        internal void InternalRemoveNode(AvlTreeNode<T> node)
        {
            _version++;
            var parent = node.Parent;
            var dleft = node.Left;
            var dright = node.Right;

            var direction = node.Direction;

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
                _attachToParentEdge(dleft, parent.EdgeTo(direction));

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
                _attachToParentEdge(dright, parent.EdgeTo(direction));

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

                // two cases. One of them is that the successor is the direct right child of the deleted node
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
                    _attachToParentEdge(dright, parent.EdgeTo(direction));
                    _attachToParentEdge(dleft, dright.EdgeLeft());
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

                    _attachToParentEdge(successor, parent.EdgeTo(direction));
                    _attachToParentEdge(dleft, successor.EdgeLeft());
                    _attachToParentEdge(dright, successor.EdgeRight());
                    _attachToParentEdge(sright, sparent.EdgeLeft());

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

        internal AvlTreeNode<T> InternalClear()
        {
            _version++;
            var root = Root;
            _dettachFromParent(Root);
            return root;
        }

        internal void InternalReset(T[] items)
        {
            InternalClear();
            var root = rec_createRangeSubtree(items, 0, items.Length - 1);
            if (root != null) _attachToParentEdge(root, AvlTreeEdge<T>.Root);
        }


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

        private IEnumerable<AvlTreeNode<T>> _enumerate(AvlTreeNode<T> node)
        {
            var res = Enumerable.Empty<AvlTreeNode<T>>();
            if (node == null) return res;

            if (node.Left != null) res = res.Concat(_enumerate(node.Left));
            res = res.Concat(node);
            if (node.Right != null) res = res.Concat(_enumerate(node.Right));

            return res;
        }

        private AvlTreeNode<T> rec_createRangeSubtree(T[] items, int start, int end)
        {
            if (start > end) return null;

            var mid = (start + end) / 2;
            var item = items[mid];

            var node = new AvlTreeNode<T>(item);
            var left = rec_createRangeSubtree(items, start, mid - 1);
            var right = rec_createRangeSubtree(items, mid + 1, end);

            if (left != null) _attachToParentEdge(left, node.EdgeLeft());
            if (right != null) _attachToParentEdge(right, node.EdgeRight());
            _recalc(node);
            return node;
        }

        private void _attachToParentEdge(AvlTreeNode<T> node, AvlTreeEdge<T> edge)
        {
            Debug.Assert(edge.IsChildOrRoot);

            // if node != null, node.parent should be free
            Debug.Assert((node == null) || (node.Parent == null));

            // if edge is child then the target should be free;
            Debug.Assert((edge.IsRoot) || (edge.Target() == null));

            if (edge.IsRoot)
            {
                _setAsRoot(node);
            } else if (edge.Direction == AvlTreeNodeDirection.Left)
            {
                edge.Source.Left = node;
                if (node != null) node.Parent = edge.Source;
            } else
            {
                edge.Source.Right = node;
                if (node != null) node.Parent = edge.Source;
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
                var direction = start.Direction;

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

            var direction = a.Direction;

            Debug.Assert(b != null);

            // a.Left remains so no need to change this
            // b.Right remains so no need to change this

            _dettachFromParent(a); // a from parent
            _dettachFromParent(b); // b from a
            _dettachFromParent(c); // c from b

            _attachToParentEdge(c, a.EdgeRight());
            _attachToParentEdge(a, b.EdgeLeft());
            _attachToParentEdge(b, parent.EdgeTo(direction));

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

            var direction = b.Direction;

            Debug.Assert(a != null);

            // a.Left remains so no need to change this
            // b.Right remains so no need to change this

            _dettachFromParent(b); // b from parent
            _dettachFromParent(a); // a from b
            _dettachFromParent(c); // c from a

            _attachToParentEdge(c, b.EdgeLeft());
            _attachToParentEdge(b, a.EdgeRight());
            _attachToParentEdge(a, parent.EdgeTo(direction));

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
