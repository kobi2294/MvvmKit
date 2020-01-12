using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class ListChange
    {
        public int FromVersion { get; }

        public static ListItemAdded<T> Added<T>(int fromVersion, int index, T item) 
            => new ListItemAdded<T>(fromVersion, index, item);

        public static ListItemRemoved<T> Removed<T>(int fromVersion, int index, T item) 
            => new ListItemRemoved<T>(fromVersion, index, item);

        public static ListItemMoved<T> Moved<T>(int fromVersion, int oldIndex, int newIndex, T item) 
            => new ListItemMoved<T>(fromVersion, oldIndex, newIndex, item);

        public static ListItemReplaced<T> Replaced<T>(int fromVersion, int index, T oldItem, T newItem) 
            => new ListItemReplaced<T>(fromVersion, index, oldItem, newItem);

        public static ListReset<T> Reset<T>(int fromVersion, ImmutableList<T> items) 
            => new ListReset<T>(fromVersion, items);

        public ListChange(int fromVersion)
        {
            FromVersion = fromVersion;
        }
    }

    public interface IListChange<T> { 
        int FromVersion { get; }
    }
}
