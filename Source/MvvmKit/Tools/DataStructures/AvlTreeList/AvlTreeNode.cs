using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public sealed class AvlTreeNode<T>
    {
        internal T _item = default;
        internal AvlTree<T> _tree = null;

        public int Height { get; internal set; }

        public int Size { get; internal set; }

        public int Balance => (Right?.Height ?? 0) - (Left?.Height ?? 0);

        public bool IsRoot => (Parent == null) && (_tree != null);

        public bool IsLeaf => (Left == null) && (Right == null);

        public AvlTreeNodeDirection Direction => _getDirection();

        public int LeftSize => Left?.Size ?? 0;
        public int RightSize => Right?.Size ?? 0;

        public AvlTree<T> Tree => _getTree();

        public AvlTreeNode<T> Parent { get; internal set; }

        public AvlTreeNode<T> Left { get; internal set; }

        public AvlTreeNode<T> Right { get; internal set; }

        public T Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                Tree?.OnItemChanged(this);                
            }
        }

        // use this method to change item without causeing side effects
        internal void _internalSetItem(T item)
        {
            _item = item;
        }

        private AvlTreeNodeDirection _getDirection()
        {
            if (Parent == null) return AvlTreeNodeDirection.None;
            if (Parent.Left == this) return AvlTreeNodeDirection.Left;
            if (Parent.Right == this) return AvlTreeNodeDirection.Right;
            throw new SystemException("Tree node relation to parent is inconsistant");
        }

        private AvlTree<T> _getTree()
        {
            if (_tree != null) return _tree;
            if (Parent != null) return Parent._getTree();
            return null;

        }

        public AvlTreeNode(T item)
        {
            _internalSetItem(item);
            Height = 1;
            Size = 1;
            Left = null;
            Right = null;
            Parent = null;
        }

        public AvlTreeNode()
        {
            _internalSetItem(default);
            Height = 1;
            Size = 1;
            Left = null;
            Right = null;
            Parent = null;
        }

        public override string ToString()
        {
            return $"Avl Node. Item = {_item}, Height = {Height}, Size = {Size}, Balance = {Balance}";
        }
    }
}
