using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public enum ChangeType
    {
        Added, 
        Removed, 
        Moved, 
        Replaced, 
        Cleared, 
        Reset
    }
}
