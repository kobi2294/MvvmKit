using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class CollectionChange
    {
        public static CollectionChangeAdd<T> Add<T>(int index, T item)
        {
            return new CollectionChangeAdd<T>(index, item);
        }

        public static CollectionChangeRemove<T> Remove<T>(int index, T item)
        {
            return new CollectionChangeRemove<T>(index, item);
        }

        public static CollectionChangeMove<T> Move<T>(int fromIndex, int toIndex, T item)
        {
            return new CollectionChangeMove<T>(fromIndex, toIndex, item);
        }

        public static CollectionChangeReplace<T> Replace<T>(int index, T fromItem, T toItem)
        {
            return new CollectionChangeReplace<T>(index, fromItem, toItem);
        }

        public static CollectionChangeClear<T> Clear<T>()
        {
            return new CollectionChangeClear<T>();
        }
    }
}
