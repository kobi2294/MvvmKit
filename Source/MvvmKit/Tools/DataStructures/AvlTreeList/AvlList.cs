using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    /// <summary>
    /// AvlList is an IList<T> implementation that is based on ordered AVL Tree. All singular operations in the list 
    /// (insert, remove, move, update, random access) are in O(log N). Enumeration is O(N). IndexOf is also O(log N) but if the same
    /// item repeats itself, than you should assume that IndexOf returns one of the indices - but neccessarily the first or last.
    /// IndicesOf(item) returns a list of all indices where the item appears, each index takes O(log N) to find, so assuming there are
    /// K instances of the the item in the list, it will take O(k log N) time to create the list
    /// </summary>
    public class AvlList<T>: IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        private OrderedAvlTree<T> _tree;
        private EditableLookup<T, AvlTreeNode<T>> _nodes;

        public int Count => _tree.Count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                var node = _tree[index];
                return node.Item;
            }
            set
            {
                var node = _tree[index];
                var oldVal = node.Item;
                _nodes.Remove(oldVal, node);
                node.Item = value;
                _nodes.Add(node.Item, node);
            }
        }

        public int IndexOf(T item)
        {
            var node = _nodes[item].FirstOrDefault();
            if (node == null) return -1;
            return _tree.IndexOf(node);
        }

        public int[] IndicesOf(T item)
        {
            var nodes = _nodes[item];
            return nodes
                .Select(n => _tree.IndexOf(n))
                .ToArray();
        }

        public AvlList()
        {
            _tree = new OrderedAvlTree<T>();
            _nodes = new EditableLookup<T, AvlTreeNode<T>>();
        }

        public AvlList(IEnumerable<T> items)
        {
            Reset(items);
        }

        public void AddFirst(T item)
        {
            var node = _tree.AddFirst(item);
            _nodes.Add(item, node);
        }

        public void AddLast(T item)
        {
            var node = _tree.AddLast(item);
            _nodes.Add(item, node);
        }

        public void Add(T item)
        {
            AddLast(item);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public void InsertAt(int index, T item)
        {
            var node = _tree.InsertAt(index, item);
            _nodes.Add(item, node);
        }

        public void Insert(int index, T item)
        {
            InsertAt(index, item);
        }

        public void AddAfter(T anchor, T item)
        {
            if (anchor == null)
                throw new ArgumentNullException(nameof(anchor));

            if (!_nodes.Contains(anchor))
                throw new InvalidOperationException("Anchor item not found in list");

            var anchorNode = _nodes[anchor].First();
            var node = _tree.AddAfter(anchorNode, item);
            _nodes.Add(item, node);                
        }

        public void AddBefore(T anchor, T item)
        {
            if (anchor == null)
                throw new ArgumentNullException(nameof(anchor));

            if (!_nodes.Contains(anchor))
                throw new InvalidOperationException("Anchor item not found in list");

            var anchorNode = _nodes[anchor].First();
            var node = _tree.AddBefore(anchorNode, item);
            _nodes.Add(item, node);
        }

        public void Reset(IEnumerable<T> items)
        {
            _tree = new OrderedAvlTree<T>(items);
            _nodes = _tree.ToEditableLookup(node => node.Item);
        }

        public T RemoveAt(int index)
        {
            var node = _tree.RemoveAt(index);
            _nodes.Remove(node.Item, node);
            return node.Item;

        }

        void IList<T>.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        public bool Remove(T item)
        {
            var node = _nodes[item].FirstOrDefault();
            if (node == null) return false;

            _tree.Remove(node);
            _nodes.Remove(node.Item, node);
            return true;
        }

        bool ICollection<T>.Remove(T item)
        {
            return Remove(item);
        }

        public void Clear()
        {
            _nodes = new EditableLookup<T, AvlTreeNode<T>>();
            _tree = new OrderedAvlTree<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            var enumerator = _tree.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current.Item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T item)
        {
            return _nodes.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int index = 0;
            foreach (var node in _tree)
            {
                array[index++] = node.Item;
            }

        }

    }
}
