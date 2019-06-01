using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class CollectionChangeMove : ICollectionChangeMove
    {
        public int FromIndex { get; }

        public int ToIndex { get; }

        public object Item { get; }

        public CollectionChangeMove(int fromIndex, int toIndex, object item)
        {
            Item = item;
            FromIndex = fromIndex;
            ToIndex = toIndex;
        }

        #region Comparing
        public override bool Equals(object obj)
        {
            if (!(obj is CollectionChangeMove)) return false;

            return this == (CollectionChangeMove)obj;
        }

        public bool Equals(CollectionChangeMove other)
        {
            return this == other;
        }

        public static bool operator ==(CollectionChangeMove rs1, CollectionChangeMove rs2)
        {
            var isnull1 = ReferenceEquals(rs1, null);
            var isnull2 = ReferenceEquals(rs2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return ((rs1.FromIndex == rs2.FromIndex) && (rs1.ToIndex == rs2.ToIndex) && (rs1.Item == rs2.Item));
        }

        public static bool operator !=(CollectionChangeMove rs1, CollectionChangeMove rs2)
        {
            return !(rs1 == rs2);
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(Item, FromIndex, ToIndex);
        }

        #endregion

    }

    public class CollectionChangeMove<T> : CollectionChangeMove, ICollectionChangeMove<T>
    {
        public new T Item => (T)base.Item;

        public CollectionChangeMove(int fromIndex, int toIndex, T item)
            : base(fromIndex, toIndex, item)
        {
        }

        public static implicit operator CollectionChangeMove<T>((int fromIndex, int toIndex, T item) value)
        {
            return new CollectionChangeMove<T>(value.fromIndex, value.toIndex, value.item);
        }

    }
}
