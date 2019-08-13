using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public sealed class AvlTree_o<T>
    {
        public AvlTreeNode_o<T> Root { get; private set; }  // Never null

        public AvlTree_o()
        {
            Clear();
        }

        public int Count
        {
            get
            {
                return Root.Size;
            }
        }

        public AvlTreeNode_o<T> this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();
                return Root.GetNodeAt(index);
            }
        }


        public AvlTreeNode_o<T> Add(T val)
        {
            return Insert(Count, val);
        }


        public AvlTreeNode_o<T> Insert(int index, T val)
        {
            if (index < 0 || index > Count)  // Different constraint than the other methods
                throw new IndexOutOfRangeException();
            if (Count == int.MaxValue)
                throw new InvalidOperationException("Maximum size reached");

            var res = Root.InsertAt(index, val);
            Root = res.self;

            return res.added;
        }


        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();
            Root = Root.RemoveAt(index);
        }


        public void Clear()
        {
            Root = AvlTreeNode_o<T>.CreateRoot();
        }


        // For unit tests.
        public void CheckStructure()
        {
            Root.CheckStructure(new HashSet<AvlTreeNode_o<T>>());
        }
    }
}
