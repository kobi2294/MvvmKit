using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class CollectionChangeRemove : ICollectionChangeRemove
    {
        public int Index { get; }

        public object Item { get; }

        public CollectionChangeRemove(int index, object item)
        {
            Item = item;
            Index = index;
        }

        #region Comparing
        public override bool Equals(object obj)
        {
            if (!(obj is CollectionChangeRemove)) return false;

            return this == (CollectionChangeRemove)obj;
        }

        public bool Equals(CollectionChangeRemove other)
        {
            return this == other;
        }

        public static bool operator ==(CollectionChangeRemove rs1, CollectionChangeRemove rs2)
        {
            var isnull1 = ReferenceEquals(rs1, null);
            var isnull2 = ReferenceEquals(rs2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return ((rs1.Index == rs2.Index) && (rs1.Item == rs2.Item));
        }

        public static bool operator !=(CollectionChangeRemove rs1, CollectionChangeRemove rs2)
        {
            return !(rs1 == rs2);
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(Item, Index);
        }

        #endregion

    }

    public class CollectionChangeRemove<T> : CollectionChangeRemove, ICollectionChangeRemove<T>
    {
        public new T Item => (T)base.Item;

        public CollectionChangeRemove(int index, T item)
            : base(index, item)
        {
        }

        public static implicit operator CollectionChangeRemove<T>((int index, T item) value)
        {
            return new CollectionChangeRemove<T>(value.index, value.item);
        }

    }
}
