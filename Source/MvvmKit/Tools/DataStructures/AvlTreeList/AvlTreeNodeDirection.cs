using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    // What is the relation of the node to it's parent. Either it is a left child, a right child or the root
    internal enum AvlTreeNodeDirection
    {
        Root,
        Left, 
        Right
    }
}
