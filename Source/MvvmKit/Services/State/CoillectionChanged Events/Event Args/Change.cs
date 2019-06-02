using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public abstract class Change
    {
        public ChangeType ChangeType { get; }

        public Change(ChangeType changeType)
        {
            ChangeType = changeType;
        }
    }
}
