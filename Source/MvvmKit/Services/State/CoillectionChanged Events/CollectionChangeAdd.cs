using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class CollectionChangeAdd : ICollectionChangeAdd
    {
        public int Index { get; }

        public object Item { get; }

        public CollectionChangeAdd(int index, object item)
        {
            Item = item;
            Index = index;
        }

        #region Comparing
        public override bool Equals(object obj)
        {
            if (!(obj is CollectionChangeAdd)) return false;

            return this == (CollectionChangeAdd)obj;
        }

        public bool Equals(CollectionChangeAdd other)
        {
            return this == other;
        }

        public static bool operator ==(CollectionChangeAdd rs1, CollectionChangeAdd rs2)
        {
            var isnull1 = ReferenceEquals(rs1, null);
            var isnull2 = ReferenceEquals(rs2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return ((rs1.Index == rs2.Index) && (rs1.Item == rs2.Item));
        }

        public static bool operator !=(CollectionChangeAdd rs1, CollectionChangeAdd rs2)
        {
            return !(rs1 == rs2);
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(Item, Index);
        }

        #endregion

    }

    public class CollectionChangeAdd<T> : CollectionChangeAdd, ICollectionChangeAdd<T>
    {
        public new T Item => (T)base.Item;

        public CollectionChangeAdd(int index, T item)
            :base(index, item)
        {
        }

        public static implicit operator CollectionChangeAdd<T>((int index, T item) value)
        {
            return new CollectionChangeAdd<T>(value.index, value.item);
        }

    }
}
