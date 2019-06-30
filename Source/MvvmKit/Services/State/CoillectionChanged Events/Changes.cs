using MvvmKit.CollectionChangeEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public static class Changes
    {
        public static ItemAdded<T> Add<T>(int index, T item, IEnumerable<T> currentItems)
        {
            return new ItemAdded<T>(index, item, currentItems);
        }

        public static ItemRemoved<T> Remove<T>(int index, T item, IEnumerable<T> currentItems)
        {
            return new ItemRemoved<T>(index, item, currentItems);
        }

        public static ItemMoved<T> Move<T>(int fromIndex, int toIndex, T item, IEnumerable<T> currentItems)
        {
            return new ItemMoved<T>(fromIndex, toIndex, item, currentItems);
        }

        public static ItemReplaced<T> Replace<T>(int index, T fromItem, T toItem, IEnumerable<T> currentItems)
        {
            return new ItemReplaced<T>(index, fromItem, toItem, currentItems);
        }

        public static Cleared<T> Clear<T>()
        {
            return new Cleared<T>();
        }

        public static Reset<T> Reset<T>(IEnumerable<T> items)
        {
            return new Reset<T>(items);
        }

        public static CollectionChanges<T> Collect<T>(this IEnumerable<IChange<T>> source)
        {
            return new CollectionChanges<T>(source);
        }
    }
}
