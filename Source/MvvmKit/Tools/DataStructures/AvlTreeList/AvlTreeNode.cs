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

        public int Balance => (Right?.Height ?? 0) - (Left?.Height ?? 0);

        public int Size { get; internal set; }

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

        public string WindowString => _windowString();

        public string _windowString()
        {
            var sb = new StringBuilder();

            var root = new List<AvlTreeNode<T>> { Parent };
            var me = new List<AvlTreeNode<T>> { this };
            var children = new List<AvlTreeNode<T>>() { Left, Right };
            var grands = new List<AvlTreeNode<T>>() { Left?.Left, Left?.Right, Right?.Left, Right?.Right };
            var ggs = new List<AvlTreeNode<T>>() {
                Left?.Left?.Left, Left?.Left?.Right,
                Left?.Right?.Left, Left?.Right?.Right,
                Right?.Left?.Left, Right?.Left?.Right,
                Right?.Right?.Left, Right?.Right?.Right,
            };

            var sroot = _arrange(_stringifyNodeList(root, 8), 0, 90);
            var sme = _arrange(_stringifyNodeList(me, 8), 0, 90);
            var schildren = _arrange(_stringifyNodeList(children, 8), 32, 90);
            var sgrands = _arrange(_stringifyNodeList(grands, 8), 12, 90);
            var sggs = _arrange(_stringifyNodeList(ggs, 8), 2, 90);

            var relationChar = '|';
            if ((Parent != null) && (Parent.Right == this)) relationChar = '\\';
            if ((Parent != null) && (Parent.Left == this)) relationChar = '/';

            var r1 = new List<string> { relationChar.ToString() };
            var r2 = new List<string> {  "/", "\\" };
            var r3 = new List<string> { "/", "\\", "/", "\\" };
            var r4 = new List<string> { "/", "\\", "/", "\\", "/", "\\", "/", "\\" };

            var sr1 = _arrange(r1, 0, 90);
            var sr2 = _arrange(r2, 36, 90);
            var sr3 = _arrange(r3, 17, 90);
            var sr4 = _arrange(r4, 8, 90);

            sb.AppendLine(sroot);
            sb.AppendLine(sr1);
            sb.AppendLine(sme);
            sb.AppendLine(sr2);
            sb.AppendLine(schildren);
            sb.AppendLine(sr3);
            sb.AppendLine(sgrands);
            sb.AppendLine(sr4);
            sb.AppendLine(sggs);

            return sb.ToString();
        }

        private List<string> _stringifyNodeList(List<AvlTreeNode<T>> list, int itemLength)
        {
            var strings = list.Select(i => i == null
                ? $"[{new string(' ', itemLength - 2)}]"
                : $"[{_centerString(i.Item.ToString(), itemLength - 2)}]").ToList();

            return strings;
        }

        private string _arrange(List<string> strings, int spaceBetween, int totalLength)
        {
            var seperator = new string(' ', spaceBetween);
            var content = string.Join(seperator, strings);

            var res = _centerString(content, totalLength);
            return res;

        }

        private string _centerString(string s, int length)
        {
            if (s.Length > length) return s.Substring(0, length);
            int leftPad = (length - s.Length) / 2;
            int rightpad = length - s.Length - leftPad;

            var res = new string(' ', leftPad) + s + new string(' ', rightpad);
            return res;
        }
    }
}
