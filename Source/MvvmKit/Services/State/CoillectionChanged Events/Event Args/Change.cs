using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public abstract class Change
    {
        public ChangeType ChangeType { get; }

        public IReadOnlyList<object> CurrentItems { get; }

        protected IReadOnlyList<T> GetCurrentItems<T>()
        {
            return CurrentItems.Cast<T>().ToReadOnly();
        }

        public Change(ChangeType changeType, IEnumerable items)
        {
            ChangeType = changeType;
            CurrentItems = items.Cast<object>().ToReadOnly();
        }
    }
}
