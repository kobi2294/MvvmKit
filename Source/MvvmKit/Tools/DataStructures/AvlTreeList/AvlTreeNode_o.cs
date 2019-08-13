using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public sealed class AvlTreeNode_o<T>
    {
        // The height of the tree rooted at this node. Empty nodes have height 0.
        // This node has height equal to max(Left.Height, Right.Height) + 1.
        public int Height { get; private set; }

        public int Balance
        {
            get
            {
                return Right.Height - Left.Height;
            }
        }

        public bool IsEmpty { get; private set; }

        public bool IsNotEmpty => !IsEmpty;

        public bool IsRoot => Parent == null;

        private T _value;
        public T Value
        {
            get
            {
                if (IsEmpty) throw new InvalidOperationException("Cannot access value of empty node");
                return _value;
            }
            set
            {
                if (IsEmpty) throw new InvalidOperationException("Cannot access value of empty node");
                _value = value;
            }
        }

        // The number of non-empty nodes in the tree rooted at this node, including this node.
        // Empty nodes have size 0. This node has size equal to Left.Size + Right.Size + 1.
        public int Size { get; private set; }

        // The root node of the left subtree.
        public AvlTreeNode_o<T> Left { get; private set; }

        // The root node of the right subtree.
        public AvlTreeNode_o<T> Right { get; private set; }

        public AvlTreeNode_o<T> Parent { get; private set; }

        private AvlTreeNode_o<T> _setParent(AvlTreeNode_o<T> newParent)
        {
            Parent = newParent;
            return this;
        }

        internal static AvlTreeNode_o<T> CreateRoot()
        {
            return new AvlTreeNode_o<T>();
        }

        // for root
        private AvlTreeNode_o()
        {
            Value = default(T);
            Height = 0;
            Size = 0;
            Left = null;
            Right = null;
            Parent = null;
            IsEmpty = true;
        }

        // For the singleton empty leaf node.
        private AvlTreeNode_o(AvlTreeNode_o<T> parent)
        {
            Value = default(T);
            Height = 0;
            Size = 0;
            Left = null;
            Right = null;
            Parent = parent;
            IsEmpty = true;
        }


        // Normal non-leaf nodes.
        private AvlTreeNode_o(T val, AvlTreeNode_o<T> parent)
        {
            Value = val;
            Height = 1;
            Size = 1;
            Left = new AvlTreeNode_o<T>(this);
            Right = new AvlTreeNode_o<T>(this);
            Parent = parent;
        }


        internal AvlTreeNode_o<T> GetNodeAt(int index)
        {
            Debug.Assert(0 <= index && index < Size);  // Automatically implies this != EmptyLeaf, because EmptyLeaf.Size == 0
            int leftSize = Left.Size;
            if (index < leftSize)
                return Left.GetNodeAt(index);
            else if (index > leftSize)
                return Right.GetNodeAt(index - leftSize - 1);
            else
                return this;
        }


        internal (AvlTreeNode_o<T> self, AvlTreeNode_o<T> added) InsertAt(int index, T obj)
        {
            (AvlTreeNode_o<T> self, AvlTreeNode_o<T> added) res = (null, null);

            Debug.Assert(0 <= index && index <= Size);
            if (IsEmpty)  // Automatically implies index == 0, because EMPTY_LEAF.Size == 0
            {
                var newNode = new AvlTreeNode_o<T>(obj, Parent);
                return (newNode, newNode); // the new node replaces me
            }
            int leftSize = Left.Size;

            if (index <= leftSize)
            {
                res = Left.InsertAt(index, obj);
                Left = res.self;
            }
            else
            {
                res = Right.InsertAt(index - leftSize - 1, obj);
                Right = res.self;
            }
            _recalc();
            var self = _doBalance();
            return (self, res.added);

        }


        internal AvlTreeNode_o<T> RemoveAt(int index)
        {
            Debug.Assert(0 <= index && index < Size);  // Automatically implies this != EmptyLeaf, because EmptyLeaf.Size == 0
            int leftSize = Left.Size;
            if (index < leftSize)
                Left = Left.RemoveAt(index);
            else if (index > leftSize)
                Right = Right.RemoveAt(index - leftSize - 1);

            else if (Left.IsEmpty && Right.IsEmpty)
                return new AvlTreeNode_o<T>(Parent);

            else if (Left.IsNotEmpty && Right.IsEmpty)
                return Left._setParent(Parent);

            else if ((Left.IsEmpty) && (Right.IsNotEmpty))
                return Right._setParent(Parent);

            else
            {
                // this is the right index, and left and right are both full
                // Find successor node. (Using the predecessor is valid too.)
                AvlTreeNode_o<T> temp = Right;
                while (temp.Left.IsNotEmpty) temp = temp.Left;

                Value = temp.Value;  // Replace value by successor
                Right = Right.RemoveAt(0);  // Remove successor node
            }
            _recalc();
            return _doBalance();
        }


        // Balances the subtree rooted at this node and returns the new root.
        private AvlTreeNode_o<T> _doBalance()
        {
            int bal = Balance;
            Debug.Assert(Math.Abs(bal) <= 2);
            AvlTreeNode_o<T> result = this;
            if (bal == -2)
            {
                Debug.Assert(Math.Abs(Left.Balance) <= 1);
                if (Left.Balance == +1)
                    Left = Left._rotateLeft();
                result = _rotateRight();
            }
            else if (bal == +2)
            {
                Debug.Assert(Math.Abs(Right.Balance) <= 1);
                if (Right.Balance == -1)
                    Right = Right._rotateRight();
                result = _rotateLeft();
            }
            Debug.Assert(Math.Abs(result.Balance) <= 1);
            return result;
        }


        /*   P            P
         *   |            |
		 *   A            B
		 *  / \          / \
		 * 0   B   ->   A   2
		 *    / \      / \
		 *   1   2    0   1
		 */
        private AvlTreeNode_o<T> _rotateLeft()
        {
            // this = A,  root = B,  parent = P
            Debug.Assert(Right.IsNotEmpty);
            AvlTreeNode_o<T> root = Right;
            AvlTreeNode_o<T> parent = Parent;

            this.Right = root.Left;
            root.Left = this;

            this.Parent = root;
            this.Right.Parent = this;
            root.Parent = parent;

            _recalc();
            root._recalc();
            return root;
        }

        /*     P          P
         *     |          |
		 *     B          A
		 *    / \        / \
		 *   A   2  ->  0   B
		 *  / \            / \
		 * 0   1          1   2
		 */
        private AvlTreeNode_o<T> _rotateRight()
        {
            // this = B,  root = A,  parent = P
            Debug.Assert(Left.IsNotEmpty);
            AvlTreeNode_o<T> root = this.Left;
            AvlTreeNode_o<T> parent = Parent;

            this.Left = root.Right;
            root.Right = this;

            this.Parent = root;
            this.Left.Parent = this;
            root.Parent = parent;

            this._recalc();
            root._recalc();
            return root;
        }


        // Needs to be called every time the left or right subtree is changed.
        // Assumes the left and right subtrees have the correct values computed already.
        private void _recalc()
        {
            Debug.Assert(this.IsNotEmpty);
            Debug.Assert(Left.Height >= 0 && Right.Height >= 0);
            Debug.Assert(Left.Size >= 0 && Right.Size >= 0);

            Height = Math.Max(Left.Height, Right.Height) + 1;
            Size = Left.Size + Right.Size + 1;

            Debug.Assert(Height>= 0 && Size >= 0);
        }

        // For unit tests, invokable by the outer class.
        internal void CheckStructure(ISet<AvlTreeNode_o<T>> visitedNodes)
        {
            if (this.IsEmpty)
                return;

            if (!visitedNodes.Add(this))
                throw new SystemException("AVL tree structure violated: Not a tree");

            Left.CheckStructure(visitedNodes);
            Right.CheckStructure(visitedNodes);

            if ((this.Left != null) && (this.Left.Parent != this))
                throw new SystemException("AVL tree structure violated: Left Parent != me");

            if ((this.Right != null) && (this.Right.Parent != this))
                throw new SystemException("AVL tree structure violated: Right Parent != me");

            if (Height != Math.Max(Left.Height, Right.Height) + 1)
                throw new SystemException("AVL tree structure violated: Incorrect cached height");

            if (Size != Left.Size + Right.Size + 1)
                throw new SystemException("AVL tree structure violated: Incorrect cached size");

            if (Math.Abs(Balance) > 1)
                throw new SystemException("AVL tree structure violated: Height imbalance");
        }


        public override string ToString()
        {
            if (IsEmpty)
                return "AvlNode: Empty";

            if (Value == null)
                return "AvlNode: null";

            return "AvlNode: " + Value;
        }

    }
}
