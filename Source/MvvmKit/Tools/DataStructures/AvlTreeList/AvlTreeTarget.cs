using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    internal class AvlTreeTarget<T>
    {
        public AvlTreeNode<T> Parent { get; set; }

        public AvlTreeNodeDirection ChildDirection { get; set; }
    }
}
