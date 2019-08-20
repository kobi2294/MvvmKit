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
        public static ItemAdded<T> Add<T>(int index, T item)
        {
            return new ItemAdded<T>(index, item);
        }

        public static ItemRemoved<T> Remove<T>(int index, T item)
        {
            return new ItemRemoved<T>(index, item);
        }

        public static ItemMoved<T> Move<T>(int fromIndex, int toIndex, T item)
        {
            return new ItemMoved<T>(fromIndex, toIndex, item);
        }

        public static ItemReplaced<T> Replace<T>(int index, T fromItem, T toItem)
        {
            return new ItemReplaced<T>(index, fromItem, toItem);
        }

        public static Cleared<T> Clear<T>()
        {
            return new Cleared<T>();
        }

        public static Reset<T> Reset<T>(IEnumerable<T> items)
        {
            return new Reset<T>(items);
        }

        public static CollectionChanges<T> Init<T>(IEnumerable<T> items)
        {
            var reset = Reset(items);
            var res = new CollectionChanges<T>(reset.Yield(), Enumerable.Empty<T>(), items);
            return res;
        }

        public static CollectionChanges<T> Collect<T>(this IChange<T> change, IEnumerable<T> oldVals, IEnumerable<T> newVals)
        {
            return new CollectionChanges<T>(change.Yield(), oldVals, newVals);
        }

        public static CollectionChanges<T> Collect<T>(this IEnumerable<IChange<T>> changes, IEnumerable<T> oldVals, IEnumerable<T> newVals)
        {
            return new CollectionChanges<T>(changes, oldVals, newVals);
        }
    }
}
